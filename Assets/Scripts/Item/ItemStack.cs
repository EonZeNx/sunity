using MLAPI.Serialization.Pooled;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ItemStack represents a stack of items within an inventory slot.
/// </summary>
[System.Serializable]
public class ItemStack
{
    #region Fields and Constructor

    public string ItemId { get; set; }
    public int Quantity { get; set; }

    public ItemStack(string id, int quantity)
    {
        ItemId = id;
        Quantity = quantity;
    }

    #endregion

    #region Item Stack Properties

    /// <summary>
    /// Query for item definition from GameManager.
    /// </summary>
    /// <returns>Item definition object.</returns>
    public Item GetItemDefinition()
    {
        var gameManager = InventoryAndInteractionManager.Instance;
        return gameManager.GetItemById(ItemId);
    }

    /// <summary>
    /// Query for item's max stack quantity.
    /// </summary>
    /// <returns>Item max stack quantity.</returns>
    public int GetItemMaxStackQuantity()
    {
        return GetItemDefinition().MaxStackSize;
    }

    /// <summary>
    /// Is this stack empty?
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return ItemId == InventoryAndInteractionManager.NULL_ITEM_ID || Quantity == 0;
    }

    #endregion

    #region Stack-to-stack interaction

    /// <summary>
    /// Perform primary functionality (i.e. left click), 
    /// when this stack is clicked with another stack.
    /// This stack will be receiving the action.
    /// </summary>
    /// <returns>Other stack, updated</returns>
    public ItemStack OnPrimaryAction(ItemStack other)
    {
        var result = other;

        if (IsEmpty() || other.IsEmpty()) // If either stacks is empty, swap the stacks
        {
            result = SwapWith(other);
        }
        else if (ItemId != other.ItemId) // If stacks are different, swap the stacks
        {
            result = SwapWith(other);
        }
        else if (ItemId == other.ItemId) // If they are the same item, try to combine the stacks
        {
            // Merge stack quantities
            Quantity += result.Quantity;
            result.Quantity = 0;

            // Place overflow into other stack
            if (Quantity > GetItemMaxStackQuantity())
            {
                var overflow = Quantity - GetItemMaxStackQuantity();
                Quantity = GetItemMaxStackQuantity();
                result.Quantity = overflow;
            }
        }

        // Empty stack update
        UpdateEmptyStack();
        other.UpdateEmptyStack();

        // Return updated other stack
        return result;
    }

    /// <summary>
    /// Perform secondary functionality (i.e. right click), 
    /// when this stack is clicked with another stack.
    /// This stack will be receiving the action.
    /// </summary>
    /// <returns>Other stack, updated</returns>
    public ItemStack OnSecondaryAction(ItemStack other)
    {
        var result = other;

        if (IsEmpty() && !other.IsEmpty()) // If this stack is empty, move one from other to this
        {
            ItemId = other.ItemId;
            Quantity += 1;
            result.Quantity -= 1;
        }
        else if (!IsEmpty() && other.IsEmpty()) // If other stack is empty, halve it.
        {
            result.ItemId = ItemId;
            var quantityToMove = Quantity / 2;
            result.Quantity += quantityToMove;
            Quantity -= quantityToMove;
        }
        else if (ItemId == other.ItemId) // If they are the same item, try to move one from other to this
        {
            if (Quantity < GetItemMaxStackQuantity())
            {
                Quantity += 1;
                result.Quantity -= 1;
            }
        }

        // Empty stack update
        UpdateEmptyStack();
        other.UpdateEmptyStack();

        // Return updated other stack
        return result;
    }

    /// <summary>
    /// Check if the stack should be a null stack, and updates it accordingly.
    /// </summary>
    /// <returns>Whether the stack was changed or not.</returns>
    public bool UpdateEmptyStack()
    {
        var isEmpty = IsEmpty();
        
        if (isEmpty)
        {
            ItemId = InventoryAndInteractionManager.NULL_ITEM_ID;
            Quantity = 0;
        }

        return isEmpty;
    }

    /// <summary>
    /// Swaps this ItemStack's properties with another ItemStack.
    /// </summary>
    /// <param name="other">Other item stack</param>
    /// <returns>Other stack, updated</returns>
    public ItemStack SwapWith(ItemStack other)
    {
        // Store original in temporary variable
        var temp = new ItemStack(ItemId, Quantity);

        // Set original to other
        ItemId = other.ItemId;
        Quantity = other.Quantity;

        // Set other to original
        other.ItemId = temp.ItemId;
        other.Quantity = temp.Quantity;

        return other;
    }

    #endregion

    #region Networking

    public static void OnSerialize(Stream stream, ItemStack instance)
    {
        using (PooledBitWriter writer = PooledBitWriter.Get(stream)) // Serialize
        {
            // Write item id and quantity
            writer.WriteStringPacked(instance.ItemId);
            writer.WriteInt32(instance.Quantity);
        }
    }

    public static ItemStack OnDeserialize(Stream stream)
    {
        using (PooledBitReader reader = PooledBitReader.Get(stream)) // Deserialize
        {
            // Read item id and quantity
            var itemStackId = reader.ReadStringPacked().ToString();
            var itemStackQuantity = reader.ReadInt32Packed();
            return new ItemStack(itemStackId, itemStackQuantity);
        }
    }

    #endregion
}

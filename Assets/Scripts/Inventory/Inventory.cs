using MLAPI.Serialization.Pooled;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Inventory is a collection of ItemStacks.
/// </summary>
public class Inventory
{
    #region Fields and Constructor

    public readonly int Rows;
    public readonly int Cols;

    private readonly ItemStack[,] ItemSlots;

    public Inventory(int Rows, int Cols)
    {
        this.Rows = Rows;
        this.Cols = Cols;

        ItemSlots = new ItemStack[Rows, Cols];

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                ItemSlots[row, col] = new ItemStack(InventoryManager.NULL_ITEM_ID, 0);
            }
        }
    }

    #endregion

    #region Inventory Manipulation

    /// <summary>
    /// Getter for inventory slot.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public ItemStack GetItemStack(int row, int col)
    {
        return ItemSlots[row, col];
    }

    /// <summary>
    /// Setter for inventory slot.
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void SetItemStack(ItemStack stack, int row, int col)
    {
        ItemSlots[row, col] = stack;
    }

    /// <summary>
    /// Item insertion. This occurs when an item is picked up.
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns></returns>
    public ItemStack InsertItemStackIntoInventory(ItemStack itemStack)
    {
        for (int row = Rows-1; row >= 0; row--) // Top row takes priority
        {
            for (int col = 0; col < Cols; col++) // Left column takes priority
            {
                // Find first valid slot
                var slot = ItemSlots[row, col];
                if ((slot.ItemId == itemStack.ItemId && slot.Quantity < slot.GetItemMaxStackQuantity()) // Item is the same type and has space
                    || slot.IsEmpty()) // Slot is empty
                {
                    slot.ItemId = itemStack.ItemId;
                    slot.Quantity += itemStack.Quantity;
                    itemStack.Quantity = 0;

                    // Calculate overflow
                    var overflow = 0;
                    if (slot.Quantity > slot.GetItemMaxStackQuantity())
                    {
                        overflow = slot.Quantity - slot.GetItemMaxStackQuantity();
                        slot.Quantity = slot.GetItemMaxStackQuantity();
                    }
                    itemStack.Quantity = overflow;

                    // If item stack is fully inserted into inventory, quit. Else, keep going.
                    itemStack.UpdateEmptyStack();
                    if (itemStack.IsEmpty())
                    {
                        return itemStack;
                    }
                }
            }
        }
        return itemStack; // Return what is left over.
    }

    #endregion

    #region Networking

    /// <summary>
    /// Serialize inventory's items and dimensions into strean.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="instance"></param>
    public static void OnSerialize(Stream stream, Inventory instance)
    {
        StringBuilder stringBuilder = new StringBuilder("Serializing inventory...");

        using (PooledBitWriter writer = PooledBitWriter.Get(stream))
        {
            // Write dimensions
            writer.WriteInt32Packed(instance.Rows);
            writer.WriteInt32Packed(instance.Cols);

            // Write each item stack to the writer
            for (var row = 0; row < instance.Rows; row++)
            {
                for (var col = 0; col < instance.Cols; col++)
                {
                    var itemStack = instance.GetItemStack(row, col);
                    ItemStack.OnSerialize(stream, itemStack);
                }
            }
        }

        Debug.Log(stringBuilder.ToString());
    }

    /// <summary>
    /// Deserialize inventory's items and dimensions from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static Inventory OnDeserialize(Stream stream)
    {
        Debug.Log("Deserializing inventory...");

        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            // Get dimensions
            var rows = reader.ReadInt32Packed();
            var cols = reader.ReadInt32Packed();

            var inventory = new Inventory(rows, cols);

            // Get each item stack and place into inventory
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var itemStack = ItemStack.OnDeserialize(stream);
                    inventory.SetItemStack(itemStack, row, col);
                }
            }
            return inventory;
        }
    }

    #endregion

    #region Debugging

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        for (var row = Rows - 1; row >= 0; row--)
        {
            for (var col = 0; col < Cols; col++)
            {
                var itemStackString = GetItemStack(row, col);
                stringBuilder.Append(col == Cols - 1 ? $"{itemStackString}": $"{itemStackString}, ");
            }

            if (row > 0) { stringBuilder.AppendLine(); }
        }

        return stringBuilder.ToString();
    }

    #endregion
}

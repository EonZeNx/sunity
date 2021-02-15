using MLAPI.Serialization.Pooled;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Inventory is a collection of ItemStacks.
/// </summary>
public class Inventory
{
    #region Fields and Constructor

    private int Rows;
    private int Cols;

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
                ItemSlots[row, col] = new ItemStack(GameManager.NULL_ITEM_ID, 0);
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
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
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

    public static void OnSerialize(Stream stream, Inventory instance)
    {
        using (PooledBitWriter writer = PooledBitWriter.Get(stream)) // Serialize
        {
            writer.WriteStringPacked("");
        }
    }

    public static Inventory OnDeserialize(Stream stream)
    {
        using (PooledBitReader reader = PooledBitReader.Get(stream)) // Deserialize
        {
            return new Inventory(1, 1);
        }
    }

    #endregion
}

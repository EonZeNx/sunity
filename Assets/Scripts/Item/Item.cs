using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Item represents the definition of an item. This is where properties such as item name, item description, and item max stack size are found.
/// This class can be extended to provide items with differing functionality, i.e. consumables, tools.
/// </summary>
public class Item
{
    #region Fields and Constructor

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxStackSize { get; set; }
    public Sprite Sprite { get; set; }

    public Item(string Id, string Name, string Description, int MaxStackSize, Sprite Sprite)
    {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.MaxStackSize = MaxStackSize;
        this.Sprite = Sprite;
    }

    #endregion

    #region Item Usage

    /// <summary>
    /// Use the item in the stack. This method is intended to be overridden.
    /// </summary>
    /// <param name="stack">Item stack containing item to be used.</param>
    /// <param name="character">Character which used the item.</param>
    /// <returns>Whether usage was successful.</returns>
    public virtual ItemStack OnUsePrimary(ItemStack stack, CharacterInventoryAndInteraction character)
    {
        Debug.Log("No item selected, primary.");
        return null;
    }

    /// <summary>
    /// Use the item in the stack with a secondary action. This method is intended to be overridden.
    /// </summary>
    /// <param name="stack">Item stack containing item to be used.</param>
    /// <param name="character">Character which used the item.</param>
    /// <returns>Whether usage was successful.</returns>
    public virtual ItemStack OnUseSecondary(ItemStack stack, CharacterInventoryAndInteraction character)
    {
        Debug.Log("No item selected, secondary.");
        return null;
    }

    #endregion
}

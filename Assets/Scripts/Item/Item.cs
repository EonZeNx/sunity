using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Item represents the definition of an item. This is where properties such as item name, item description, and item max stack size are found.
/// This class can be extended to provide items with differing functionality, i.e. consumables, tools.
/// </summary>
public class Item
{
    #region Fields and Constructor

    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxStackSize { get; set; }

    public Item(string Name, string Description, int MaxStackSize)
    {
        this.Name = Name;
        this.Description = Description;
        this.MaxStackSize = MaxStackSize;
    }

    #endregion
}

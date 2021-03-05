using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAPI;

/// <summary>
/// Item represents the definition of an item. This is where properties such as item name, item description, and item max stack size are found.
/// This class can be extended to provide items with differing functionality, i.e. consumables, tools.
/// </summary>
[System.Serializable]
public class Item
{
    public static readonly float DEFAULT_THROW_STRENGTH = 10f;
    public static readonly float DEFAULT_START_DISTANCE = 1f;

    #region Fields and Constructor

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxStackSize { get; set; }
    public Sprite Sprite { get; set; }

    public GameObject InteractablePrefab { get; set; }

    public Item(string Id, string Name, string Description, int MaxStackSize, Sprite Sprite, GameObject InteractablePrefab)
    {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.MaxStackSize = MaxStackSize;
        this.Sprite = Sprite;
        this.InteractablePrefab = InteractablePrefab;
    }

    #endregion

    #region Item Usage

    /// <summary>
    /// Use the item in the stack. This method is intended to be overridden.
    /// </summary>
    /// <param name="stack">Item stack containing item to be used.</param>
    /// <param name="character">Character which used the item.</param>
    /// <returns>Whether usage was successful.</returns>
    public virtual ItemStack OnUsePrimary(ItemStack stack, EntityInventory character)
    {
        return null;
    }

    /// <summary>
    /// Use the item in the stack with a secondary action. This method is intended to be overridden.
    /// </summary>
    /// <param name="stack">Item stack containing item to be used.</param>
    /// <param name="character">Character which used the item.</param>
    /// <returns>Whether usage was successful.</returns>
    public virtual ItemStack OnUseSecondary(ItemStack stack, EntityInventory character)
    {
        return null;
    }

    /// <summary>
    /// Thow the item. This method can be overridden.
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public virtual ItemStack OnThrow(ItemStack stack, EntityInventory character)
    {
        Debug.Log("Throwing!");
        if (!stack.IsEmpty())
        {
            // Decrement stack
            var itemStack = new ItemStack(stack.ItemId, stack.Quantity - 1);

            // Spawn item in front of character
            var itemInteractable = Object.Instantiate(InteractablePrefab);
            itemInteractable.transform.position = character.transform.position + character.transform.forward * DEFAULT_START_DISTANCE;
            itemInteractable.transform.rotation = character.transform.rotation;

            itemInteractable.GetComponent<NetworkedObject>().Spawn();
            // Add force to make item go forward
            var initialForce = character.transform.forward * DEFAULT_THROW_STRENGTH;
            itemInteractable.GetComponent<Rigidbody>().AddForce(initialForce, ForceMode.Impulse);

            // Check if item stack empty
            itemStack.UpdateEmptyStack();
            return itemStack;
        }
        return stack;
    }

    #endregion
}

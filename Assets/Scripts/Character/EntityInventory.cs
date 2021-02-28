using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum HotbarItemStackActionType
{
    Throw, // throw an item on the floor
    Primary, // primary action e.g. shooting a gun
    Secondary // secondary action e.g. aiming a gun
} 

/// <summary>
/// CharacterInventoryAndInteraction handles:
/// - Item interaction and inventory
/// - World interaction
/// </summary>
public class EntityInventory : NetworkedBehaviour
{
    #region Constructor and Variables

    [Header("References")]
    private readonly NetworkedVar<Inventory> MainInventory = new NetworkedVar<Inventory>(
        new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone },
        new Inventory(4, 10));
    private readonly NetworkedVar<Inventory> HotbarInventory = new NetworkedVar<Inventory>(
        new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone },
        new Inventory(1, 10));
    private readonly NetworkedVar<ItemStack> MouseSlot = new NetworkedVar<ItemStack>(
        new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone },
        new ItemStack(InventoryManager.NULL_ITEM_ID, 0));

    [Header("Prefabs")]
    public GameObject itemInteractable;

    #endregion

    #region Input Events

    /// <summary>
    /// Player presses Inventory key. 
    /// Opens inventory.
    /// </summary>
    /// <param name="input"></param>
    public void OnInventory(InputValue _)
    {
        InventoryManager.Singleton.InventoryUI.ToggleInventory();
    }

    /// <summary>
    /// Player presses throw key. Will throw out one of selected item stack.
    /// </summary>
    /// <param name="inputValue"></param>
    public void OnThrow(InputValue inputValue)
    {
        if (inputValue.Get<float>() <= 0.1f)
        {
            return;
        }
        if (!InventoryManager.Singleton.InventoryUI.mainInventoryOpen)
        {
            PerformSelectedItemStackAction(HotbarItemStackActionType.Throw);
        }
    }

    /// <summary>
    /// Player presses Primary Action key.
    /// Performs primary action on selected hotbar item.
    /// </summary>
    /// <param name="inputValue"></param>
    public void OnPrimaryAction(InputValue inputValue)
    {
        if (inputValue.Get<float>() <= 0.1f)
        {
            return;
        }
        if (!InventoryManager.Singleton.InventoryUI.mainInventoryOpen)
        {
            PerformSelectedItemStackAction(HotbarItemStackActionType.Primary);
        }
    }

    /// <summary>
    /// Player presses Secondary Action key.
    /// Performs secondary action on selected hotbar item.
    /// </summary>
    /// <param name="inputValue"></param>
    public void OnSecondaryAction(InputValue inputValue)
    {
        if (inputValue.Get<float>() <= 0.1f)
        {
            return;
        }
        if (!InventoryManager.Singleton.InventoryUI.mainInventoryOpen)
        {
            PerformSelectedItemStackAction(HotbarItemStackActionType.Secondary);
        }
    }

    /// <summary>
    /// Player scrolls with HotbarScroll input.
    /// Toggle selected item in hotbar.
    /// </summary>
    /// <param name="scroll"></param>
    public void OnHotbarScroll(InputValue scroll)
    {
        var scrollValue = scroll.Get<Vector2>().y;
        if (scrollValue > 20)
        {
            InventoryManager.Singleton.HotbarUI.NavigateToLeft();
        }
        else if (scrollValue < -20)
        {
            InventoryManager.Singleton.HotbarUI.NavigateToRight();
        }
    }

    #endregion

    #region External Actions

    /// <summary>
    /// Player performs an action on a selected item.
    /// </summary>
    /// <param name="actionType"></param>
    [ServerRPC]
    public void PerformSelectedItemStackAction(HotbarItemStackActionType actionType)
    {
        var itemStack = InventoryManager.Singleton.HotbarUI.GetSelectedItemStack();
        switch (actionType)
        {
            case HotbarItemStackActionType.Primary:
                itemStack = itemStack.GetItemDefinition().OnUsePrimary(itemStack, this);
                break;
            case HotbarItemStackActionType.Secondary:
                itemStack = itemStack.GetItemDefinition().OnUseSecondary(itemStack, this);
                break;
            case HotbarItemStackActionType.Throw:
                
                break;
        }
        
        if (itemStack != null)
        {
            InventoryManager.Singleton.HotbarUI.SetSelectedItemStack(itemStack);
        }

        MarkAllInventoriesAsDirty();
    }

    #endregion

    #region Inventory Manipulation

    public Inventory GetMainInventory()
    {
        return MainInventory.Value;
    }

    public Inventory GetHotbarInventory()
    {
        return HotbarInventory.Value;
    }

    public ItemStack GetMouseSlot()
    {
        return MouseSlot.Value;
    }

    /// <summary>
    /// Insert an item stack into this character's overall inventory.
    /// Hotbar takes priority, then Main Inventory.
    /// </summary>
    /// <param name="stack"></param>
    /// <returns></returns>
    [ServerRPC(RequireOwnership = false)]
    public ItemStack PickupItemStack(ItemStack stack)
    {
        Debug.Log($"Attempting to insert {stack} into Player {OwnerClientId}'s inventory.");
        var overflow = HotbarInventory.Value.InsertItemStackIntoInventory(stack);
        overflow = MainInventory.Value.InsertItemStackIntoInventory(overflow);

        MarkAllInventoriesAsDirty();

        return overflow;
    }

    #endregion

    #region Networking

    public void MarkAllInventoriesAsDirty()
    {
        HotbarInventory.isDirty = true;
        MainInventory.isDirty = true;
        MouseSlot.isDirty = true;
    }

    #endregion

    #region Dev Commands

    public void GiveBandages()
    {
        PickupItemStack(new ItemStack("bandage", 20));
    }

    #endregion
}

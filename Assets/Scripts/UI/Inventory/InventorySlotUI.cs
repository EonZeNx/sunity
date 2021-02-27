using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// InventorySlotUI is a single inventory slot. Can be clicked, which will perform inventory stack manipulation.
/// </summary>
public class InventorySlotUI : SlotUI, IPointerClickHandler
{
    #region Fields and Constructor

    public int Row { get; set; }
    public int Col { get; set; }
    public Inventory Inventory { get; set; }

    public bool Clickable = true;

    #endregion

    #region Unity Lifecycle Methods

    // Start is called before the first frame update
    void Start()
    {
        ItemStack = new ItemStack(InventoryManager.NULL_ITEM_ID, 0);
    }

    void Update()
    {
        UpdateItemStack();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Set image and text
        ItemImage.sprite = ItemStack.GetItemDefinition().Sprite;
        ItemQuantityText.text = ItemStack.Quantity == 0 || ItemStack.Quantity == 1 ? "": ItemStack.Quantity.ToString();
    }

    void UpdateItemStack()
    {
        ItemStack = Inventory.GetItemStack(Row, Col);
    }

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Clickable)
        {
            // Get player mouse item slot, and perform action
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    PrimaryAction();
                    break;
                case PointerEventData.InputButton.Right:
                    SecondaryAction();
                    break;
                case PointerEventData.InputButton.Middle:
                    InsertBandagesIntoInventory();
                    break;
                default:
                    break;
            }
        }
    }

    public void PrimaryAction()
    {
        var playerInventory = PlayerManager.Singleton.LocalPlayerInventory;

        var resultStack = Inventory.GetItemStack(Row, Col).OnPrimaryAction(playerInventory.GetMouseSlot());
        playerInventory.GetMouseSlot().ItemId = resultStack.ItemId;
        playerInventory.GetMouseSlot().Quantity = resultStack.Quantity;
    }

    public void SecondaryAction()
    {
        var playerInventory = PlayerManager.Singleton.LocalPlayerInventory;

        var resultStack = Inventory.GetItemStack(Row, Col).OnSecondaryAction(playerInventory.GetMouseSlot());
        playerInventory.GetMouseSlot().ItemId = resultStack.ItemId;
        playerInventory.GetMouseSlot().Quantity = resultStack.Quantity;
    }

    /// <summary>
    /// Spawns bandages. This is used for debugging purposes.
    /// </summary>
    public void InsertBandagesIntoInventory()
    {
        Inventory.InsertItemStackIntoInventory(new ItemStack("bandage", 20));
    }
}

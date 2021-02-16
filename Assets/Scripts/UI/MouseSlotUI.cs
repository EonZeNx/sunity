using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseSlotUI: SlotUI
{
    [Header("Mouse Position Offset")]
    public int MouseXOffset = 0;
    public int MouseYOffset = 0;

    [HideInInspector]
    public CharacterInventory CharacterInventory;

    public void Start()
    {
        CharacterInventory = GameManager.Instance.GetLocalPlayerInventory();
    }

    public void Update()
    {
        ItemStack = CharacterInventory.MouseSlot;
    }

    public void LateUpdate()
    {
        // This slot follows the mouse
        var mouseX = Mouse.current.position.x.ReadValue();
        var mouseY = Mouse.current.position.y.ReadValue();

        var mouseSlotTransform = GetComponent<RectTransform>();
        mouseSlotTransform.position = new Vector2(mouseX + MouseXOffset, mouseY + MouseYOffset);

        // Set image and text
        ItemImage.sprite = ItemStack.GetItemDefinition().Sprite;
        ItemQuantityText.text = ItemStack.Quantity == 0 || ItemStack.Quantity == 1 ? "" : ItemStack.Quantity.ToString();
    }
}

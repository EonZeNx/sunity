using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HotbarInventoryUI represents a UI component for a hotbar inventory. 
/// A hotbar inventory is a collection of inventory slots which can be scrolled through.
/// </summary>
public class HotbarInventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject HotbarPointer;

    [Header("Prefabs")]
    public GameObject InventorySlotPrefab;

    [Header("UI")]
    public float OverallWidthOffset = -77;
    public float OverallHeightOffset = 9;
    public float SlotWidth = 14;
    public float SlotHeight = 14;
    public float SlotSpacingWidth = 3;
    public float SlotSpacingHeight = 3;
    public float PointerWidthOffset = -84.5f;
    public float PointerHeightOffset = 0;

    [Tooltip("This should only be on if you want to dynamically change UI positioning through the editor. Otherwise, leave this off for performance.")]
    public bool ShouldUpdateSlotPositions = false;

    [Header("Hotbar State")]
    public int SelectedSlotIndex;

    private CharacterInventoryAndInteraction CharacterInventory;
    private IList<InventorySlotUI> HotbarInventorySlots;

    // Start is called before the first frame update
    void Start()
    {
        SelectedSlotIndex = 0;
        HotbarInventorySlots = new List<InventorySlotUI>();

        CharacterInventory = GameManager.Instance.GetLocalPlayerInventory();
        var hotbarInventory = CharacterInventory.HotbarInventory;

        // Create hotbar inventory slots
        for (int row = 0; row < hotbarInventory.Rows; row++)
        {
            for (int col = 0; col < hotbarInventory.Cols; col++)
            {
                var slot = Instantiate(InventorySlotPrefab, transform)
                    .GetComponent<InventorySlotUI>();
                slot.name = $"Slot: Row {row}, Col {col}";
                slot.Row = row;
                slot.Col = col;
                slot.Inventory = hotbarInventory;
                slot.Clickable = false;
                HotbarInventorySlots.Add(slot);
            }
        }

        UpdateHotbarSlotPositions();
        UpdateHotbarPointerPosition();
    }

    public void NavigateToRight()
    {
        var minIndex = 0;
        var maxIndex = HotbarInventorySlots.Count - 1;
        SelectedSlotIndex++;
        if(SelectedSlotIndex > maxIndex)
        {
            SelectedSlotIndex = minIndex;
        }

        UpdateHotbarPointerPosition();
    }

    public void NavigateToLeft()
    {
        var minIndex = 0;
        var maxIndex = HotbarInventorySlots.Count - 1;
        SelectedSlotIndex--;
        if(SelectedSlotIndex < minIndex)
        {
            SelectedSlotIndex = maxIndex;
        }

        UpdateHotbarPointerPosition();
    }

    public void UpdateHotbarSlotPositions()
    {
        // Hotbar slots
        foreach (var slot in HotbarInventorySlots)
        {
            slot.transform.localPosition = new Vector3(
                slot.Col * (SlotWidth + SlotSpacingWidth) + OverallWidthOffset,
                slot.Row * (SlotHeight + SlotSpacingHeight) + OverallHeightOffset,
                0);
        }
    }

    public void UpdateHotbarPointerPosition()
    {
        HotbarPointer.transform.localPosition = new Vector3(
            SelectedSlotIndex * (SlotWidth + SlotSpacingWidth) + PointerWidthOffset, 
            PointerHeightOffset, 
            0);
    }

    public ItemStack GetSelectedItemStack()
    {
        return CharacterInventory.HotbarInventory.GetItemStack(0, SelectedSlotIndex);
    }

    public void SetSelectedItemStack(ItemStack stack)
    {
        CharacterInventory.HotbarInventory.SetItemStack(stack, 0, SelectedSlotIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (ShouldUpdateSlotPositions)
        {
            UpdateHotbarSlotPositions();
        }
    }
}

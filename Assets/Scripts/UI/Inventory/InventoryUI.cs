using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Inventory UI is an inventory consisting of multiple inventory slots that can be interacted with by clicking.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    #region Singleton

    private static InventoryUI _instance;

    public static InventoryUI Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    [Header("References")]
    public GameObject InventoryPanel;

    [Header("Prefabs")]
    public GameObject InventorySlotPrefab;
    public GameObject MouseSlotPrefab;

    [Header("Inventory State")]
    public bool mainInventoryOpen;

    [Header("UI")]
    public float OverallWidthOffset = -77;
    public float OverallHeightOffset = -13;
    public float SlotWidth = 14;
    public float SlotHeight = 14;
    public float SlotSpacingWidth = 3;
    public float SlotSpacingHeight = 3;
    public float HotbarHeightOffset = -25;
    [Tooltip("This should only be on if you want to dynamically change UI positioning through the editor. Otherwise, leave this off for performance.")]
    public bool ShouldUpdateSlotPositions = false;

    private EntityInventory CharacterInventory;
    private IList<InventorySlotUI> MainInventorySlots;
    private IList<InventorySlotUI> HotbarInventorySlots;
    private MouseSlotUI MouseInventorySlot;

    // Start is called before the first frame update
    public void Init()
    {
        MainInventorySlots = new List<InventorySlotUI>();
        HotbarInventorySlots = new List<InventorySlotUI>();

        CharacterInventory = PlayerManager.Singleton.LocalPlayerInventory;
        var mainInventory = CharacterInventory.GetMainInventory();
        var hotbarInventory = CharacterInventory.GetHotbarInventory();
        var mouseStack = CharacterInventory.GetMouseSlot();

        // Create main inventory slots
        for (int row = 0; row < mainInventory.Rows; row++)
        {
            for (int col = 0; col < mainInventory.Cols; col++)
            {
                var slot = Instantiate(InventorySlotPrefab, InventoryPanel.transform)
                    .GetComponent<InventorySlotUI>();
                slot.name = $"Slot: Row {row}, Col {col}";
                slot.Row = row;
                slot.Col = col;
                slot.Inventory = mainInventory;
                MainInventorySlots.Add(slot);
            }
        }

        // Create hotbar inventory slots
        for (int row = 0; row < hotbarInventory.Rows; row++)
        {
            for (int col = 0; col < hotbarInventory.Cols; col++)
            {
                var slot = Instantiate(InventorySlotPrefab, InventoryPanel.transform)
                    .GetComponent<InventorySlotUI>();
                slot.name = $"Slot: Row {row}, Col {col}";
                slot.Row = row;
                slot.Col = col;
                slot.Inventory = hotbarInventory;
                HotbarInventorySlots.Add(slot);
            }
        }

        // Create mouse inventory slot
        var mouseSlot = Instantiate(MouseSlotPrefab, InventoryPanel.transform)
            .GetComponent<MouseSlotUI>();
        mouseSlot.name = $"Mouse Slot";
        mouseSlot.ItemStack = mouseStack;
        MouseInventorySlot = mouseSlot;

        UpdateInventorySlotPositions();
    }

    public void UpdateReferences()
    {
        CharacterInventory = PlayerManager.Singleton.LocalPlayerInventory;
        var mainInventory = CharacterInventory.GetMainInventory();
        var hotbarInventory = CharacterInventory.GetHotbarInventory();

        // Main Inventory slots
        foreach(var slot in MainInventorySlots)
        {
            slot.Inventory = mainInventory;
        }

        // Hotbar slots
        foreach(var slot in HotbarInventorySlots)
        {
            slot.Inventory = hotbarInventory;
        }

        // Mouse slot
        MouseInventorySlot.CharacterInventory = CharacterInventory;
    }

    /// <summary>
    /// Update the local XYZ positioning of inventory slots, based on their positioning.
    /// </summary>
    void UpdateInventorySlotPositions()
    {
        // Main inventory slots
        foreach (var slot in MainInventorySlots)
        {
            slot.transform.localPosition = new Vector3(
                slot.Col * (SlotWidth + SlotSpacingWidth) + OverallWidthOffset,
                slot.Row * (SlotHeight + SlotSpacingHeight) + OverallHeightOffset,
                0);
        }

        // Hotbar slots
        foreach (var slot in HotbarInventorySlots)
        {
            slot.transform.localPosition = new Vector3(
                slot.Col * (SlotWidth + SlotSpacingWidth) + OverallWidthOffset,
                slot.Row * (SlotHeight + SlotSpacingHeight) + OverallHeightOffset + HotbarHeightOffset,
                0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateReferences();

        // Open or close inventory based on MainInventoryOpen
        if (mainInventoryOpen && !InventoryPanel.activeSelf)
        {
            InventoryPanel.SetActive(true);
        }
        else if (!mainInventoryOpen && InventoryPanel.activeSelf)
        {
            InventoryPanel.SetActive(false);
        }
    }

    void LateUpdate()
    {
        // Update slot positions
        if (ShouldUpdateSlotPositions)
        {
            UpdateInventorySlotPositions();
        }
    }

    public void ToggleInventory()
    {
        mainInventoryOpen = !mainInventoryOpen;
    }
}

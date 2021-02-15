using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CharacterInventoryBehaviour handles interfacing with player inventory, and player inventory/item interaction.
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    public readonly Inventory MainInventory;
    public readonly ItemStack MouseSlot;

    public CharacterInventory(): base()
    {
        MainInventory = new Inventory(5, 10);
        MouseSlot = new ItemStack(GameManager.NULL_ITEM_ID, 0);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Rendering
    private void LateUpdate()
    {

    }

    public void OnInventory()
    {
        InventoryUI.Instance.ToggleInventory();
    } 
}

using MLAPI;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CharacterInventoryAndInteraction handles:
/// - Item interaction and inventory
/// - World interaction
/// </summary>
public class CharacterInventoryAndInteraction : NetworkedBehaviour
{
    #region Constructor and Variables

    [Header("References")]
    private readonly NetworkedVar<Inventory> MainInventory = new NetworkedVar<Inventory>(new Inventory(4, 10));
    private readonly NetworkedVar<Inventory> HotbarInventory = new NetworkedVar<Inventory>(new Inventory(1, 10));
    private readonly NetworkedVar<ItemStack> MouseSlot = new NetworkedVar<ItemStack>(new ItemStack(InventoryAndInteractionManager.NULL_ITEM_ID, 0));

    [Header("Interaction")]
    public Camera MainCamera;
    public float InteractionDistance = 20.0f;
    public Interactable InteractableInRange;

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        // Interaction
        if (!InventoryAndInteractionManager.Instance.InventoryUI.mainInventoryOpen)
        {
            // TODO: Seperate raycast from object detection.
            var lookDirection = MainCamera.transform.forward.normalized;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(MainCamera.transform.position, lookDirection, out RaycastHit hit, InteractionDistance))
            {
                Debug.DrawRay(MainCamera.transform.position, lookDirection * hit.distance, Color.yellow);

                var interactable = hit.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    InteractableInRange = interactable;
                }
            }
            else
            {
                Debug.DrawRay(MainCamera.transform.position, lookDirection, Color.blue);

                InteractableInRange = null;
            }
        }
    }

    // Rendering
    private void LateUpdate()
    {
        if(InteractableInRange != null)
        {
            InventoryAndInteractionManager.Instance.InteractionText.text = InteractableInRange.DisplayName + " [Press F to interact]";
        } else
        {
            InventoryAndInteractionManager.Instance.InteractionText.text = "";
        }
    }

    #endregion

    #region Input Events

    /// <summary>
    /// Player presses Inventory key. 
    /// Opens inventory.
    /// </summary>
    /// <param name="input"></param>
    public void OnInventory(InputValue _)
    {
        InventoryAndInteractionManager.Instance.InventoryUI.ToggleInventory();
    }
    
    /// <summary>
    /// Player presses Primary Action key.
    /// Performs primary action on selected hotbar item.
    /// </summary>
    /// <param name="input"></param>
    public void OnPrimaryAction(InputValue _)
    {
        if (!InventoryAndInteractionManager.Instance.InventoryUI.mainInventoryOpen)
        {
            var itemStack = InventoryAndInteractionManager.Instance.HotbarUI.GetSelectedItemStack();
            var newStack = itemStack.GetItemDefinition().OnUsePrimary(itemStack, this);
            if (newStack != null)
            {
                InventoryAndInteractionManager.Instance.HotbarUI.SetSelectedItemStack(newStack);
            }
        }
    }

    /// <summary>
    /// Player presses Secondary Action key.
    /// Performs secondary action on selected hotbar item.
    /// </summary>
    /// <param name="input"></param>
    public void OnSecondaryAction(InputValue _)
    {
        if (!InventoryAndInteractionManager.Instance.InventoryUI.mainInventoryOpen)
        {
            var itemStack = InventoryAndInteractionManager.Instance.HotbarUI.GetSelectedItemStack();
            var newStack = itemStack.GetItemDefinition().OnUseSecondary(itemStack, this);
            if (newStack != null)
            {
                InventoryAndInteractionManager.Instance.HotbarUI.SetSelectedItemStack(newStack);
            }
        }
    }

    /// <summary>
    /// Player presses Interact key.
    /// Interacts with interactable in the world.
    /// </summary>
    /// <param name="input"></param>
    public void OnInteract(InputValue _)
    {
        if(InteractableInRange != null)
        {
            InteractableInRange.Interact(this);
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
            InventoryAndInteractionManager.Instance.HotbarUI.NavigateToLeft();
        }
        else if (scrollValue < -20)
        {
            InventoryAndInteractionManager.Instance.HotbarUI.NavigateToRight();
        }
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
    public ItemStack PickupItemStack(ItemStack stack)
    {
        var overflow = HotbarInventory.Value.InsertItemStackIntoInventory(stack);
        overflow = MainInventory.Value.InsertItemStackIntoInventory(overflow);
        return overflow;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CharacterInventoryBehaviour handles interfacing with player inventory, and player inventory/item interaction.
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    [Header("References")]
    public readonly Inventory MainInventory;
    public readonly Inventory HotbarInventory;
    public readonly ItemStack MouseSlot;

    [Header("Interaction")]
    public Camera MainCamera;
    public float InteractionDistance = 20.0f;
    public Interactable InteractableInRange;

    public CharacterInventory(): base()
    {
        MainInventory = new Inventory(4, 10);
        HotbarInventory = new Inventory(1, 10);
        MouseSlot = new ItemStack(GameManager.NULL_ITEM_ID, 0);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.InventoryUI.MainInventoryOpen)
        {
            RaycastHit hit;
            var lookDirection = MainCamera.transform.forward.normalized;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(MainCamera.transform.position, lookDirection, out hit, InteractionDistance))
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
            GameManager.Instance.InteractionText.text = InteractableInRange.DisplayName + " [Press F to interact]";
        } else
        {
            GameManager.Instance.InteractionText.text = "";
        }
    }

    #region Input Events

    public void OnInventory(InputValue input)
    {
        InventoryUI.Instance.ToggleInventory();
    }

    public void OnPrimaryAction(InputValue input)
    {
        if (!GameManager.Instance.InventoryUI.MainInventoryOpen)
        {
            var itemStack = GameManager.Instance.HotbarUI.GetSelectedItemStack();
            var newStack = itemStack.GetItemDefinition().OnUsePrimary(itemStack, this);
            if (newStack != null)
            {
                GameManager.Instance.HotbarUI.SetSelectedItemStack(newStack);
            }
        }
    }

    public void OnSecondaryAction(InputValue input)
    {
        if (!GameManager.Instance.InventoryUI.MainInventoryOpen)
        {
            var itemStack = GameManager.Instance.HotbarUI.GetSelectedItemStack();
            var newStack = itemStack.GetItemDefinition().OnUseSecondary(itemStack, this);
            if (newStack != null)
            {
                GameManager.Instance.HotbarUI.SetSelectedItemStack(newStack);
            }
        }
    }

    public void OnInteract(InputValue input)
    {
        if(InteractableInRange != null)
        {
            InteractableInRange.Interact(this);
        }
    }

    /// <summary>
    /// Toggle selected item in hotbar.
    /// </summary>
    /// <param name="scroll"></param>
    public void OnHotbarScroll(InputValue scroll)
    {
        var scrollValue = scroll.Get<Vector2>().y;
        if (scrollValue > 20)
        {
            GameManager.Instance.HotbarUI.NavigateToLeft();
        }
        else if (scrollValue < -20)
        {
            GameManager.Instance.HotbarUI.NavigateToRight();
        }
    }

    #endregion

    public ItemStack PickupItemStack(ItemStack stack)
    {
        var overflow = HotbarInventory.InsertItemStackIntoInventory(stack);
        overflow = MainInventory.InsertItemStackIntoInventory(overflow);
        return overflow;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CharacterInventoryAndInteraction handles:
/// - Item interaction and inventory
/// - World interaction
/// </summary>
public class CharacterInventoryAndInteraction : MonoBehaviour
{
    #region Constructor and Variables

    [Header("References")]
    public readonly Inventory MainInventory;
    public readonly Inventory HotbarInventory;
    public readonly ItemStack MouseSlot;

    [Header("Interaction")]
    public Camera MainCamera;
    public float InteractionDistance = 20.0f;
    public Interactable InteractableInRange;

    public CharacterInventoryAndInteraction(): base()
    {
        MainInventory = new Inventory(4, 10);
        HotbarInventory = new Inventory(1, 10);
        MouseSlot = new ItemStack(GameManager.NULL_ITEM_ID, 0);
    }

    #endregion

    #region Unity Lifecycle Methods

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Interaction
        if (!GameManager.Instance.InventoryUI.MainInventoryOpen)
        {
            // TODO: Seperate raycast from object detection.
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

    #endregion

    #region Input Events

    /// <summary>
    /// Player presses Inventory key. 
    /// Opens inventory.
    /// </summary>
    /// <param name="input"></param>
    public void OnInventory(InputValue input)
    {
        InventoryUI.Instance.ToggleInventory();
    }
    
    /// <summary>
    /// Player presses Primary Action key.
    /// Performs primary action on selected hotbar item.
    /// </summary>
    /// <param name="input"></param>
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

    /// <summary>
    /// Player presses Secondary Action key.
    /// Performs secondary action on selected hotbar item.
    /// </summary>
    /// <param name="input"></param>
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

    /// <summary>
    /// Player presses Interact key.
    /// Interacts with interactable in the world.
    /// </summary>
    /// <param name="input"></param>
    public void OnInteract(InputValue input)
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
            GameManager.Instance.HotbarUI.NavigateToLeft();
        }
        else if (scrollValue < -20)
        {
            GameManager.Instance.HotbarUI.NavigateToRight();
        }
    }

    #endregion

    #region Inventory Manipulation

    /// <summary>
    /// Insert an item stack into this character's overall inventory.
    /// Hotbar takes priority, then Main Inventory.
    /// </summary>
    /// <param name="stack"></param>
    /// <returns></returns>
    public ItemStack PickupItemStack(ItemStack stack)
    {
        var overflow = HotbarInventory.InsertItemStackIntoInventory(stack);
        overflow = MainInventory.InsertItemStackIntoInventory(overflow);
        return overflow;
    }

    #endregion
}

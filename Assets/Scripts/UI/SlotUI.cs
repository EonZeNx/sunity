using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This is an inventory UI slot. Should be extended to provide more extensive functionality.
/// </summary>
public class SlotUI : MonoBehaviour
{
    #region Fields and Constructor

    [Header("References")]
    public Image ItemImage;
    public Text ItemQuantityText;
    public ItemStack ItemStack { get; set; }

    #endregion

    #region Unity Lifecycle Methods

    // Start is called before the first frame update
    void Start()
    {
        ItemStack = new ItemStack(GameManager.NULL_ITEM_ID, 0);
    }

    void LateUpdate()
    {
        // Set image and text
        ItemImage.sprite = ItemStack.GetItemDefinition().Sprite;
        ItemQuantityText.text = ItemStack.Quantity == 0 || ItemStack.Quantity == 1 ? "" : ItemStack.Quantity.ToString();
    }

    #endregion
}

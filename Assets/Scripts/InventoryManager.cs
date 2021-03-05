using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InventoryAndInteractionManager contains global information about item/interaction UI elements and state, as well as helper methods.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    #region Singleton

    private static InventoryManager _singleton;

    public static InventoryManager Singleton { get { return _singleton; } }

    private void Awake()
    {
        if (_singleton != null && _singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _singleton = this;
        }
    }

    #endregion

    #region Constants

    public static readonly string NULL_ITEM_ID = "null";

    #endregion

    #region Variables

    public readonly IList<Item> ItemDefinitions = new List<Item>();

    #endregion

    #region UI

    public InventoryUI InventoryUI;
    public HotbarInventoryUI HotbarUI;

    #endregion

    #region Item and Inventory

    /// <summary>
    /// Load all item sprites and prefabs into memory.
    /// </summary>
    void InitItemAndInventory()
    {
        var ItemSprites = Resources.LoadAll<Sprite>("Item/Sprite/items");
        var ItemPrefabs = Resources.LoadAll<GameObject>("Item/Prefab/");

        ItemDefinitions.Add(new Item(
            "null",
            "Null",
            "Used to represent empty space.",
            0,
            ItemSprites.SingleOrDefault(sprite => sprite.name == "null"),
            ItemPrefabs.SingleOrDefault(prefab => prefab.name == "null")
            ));
        ItemDefinitions.Add(new Item(
            "knife", 
            "Hunting Knife", 
            "A sharp blade used for hunting and utility.", 
            1,
            ItemSprites.SingleOrDefault(sprite => sprite.name == "knife"),
            ItemPrefabs.SingleOrDefault(prefab => prefab.name == "knife")
            ));
        ItemDefinitions.Add(new HealingItem(
            "bandage", 
            "Bandage", 
            "Bandages will give a small HP revovery.", 
            20,
            ItemSprites.SingleOrDefault(sprite => sprite.name == "bandage"),
            ItemPrefabs.SingleOrDefault(prefab => prefab.name == "bandage")
            ));
    }

    /// <summary>
    /// Needed to allow serialization for item stacks and inventory.
    /// </summary>
    void InitItemAndInventoryNetworking()
    {
        SerializationManager.RegisterSerializationHandlers(Inventory.OnSerialize, Inventory.OnDeserialize);
        SerializationManager.RegisterSerializationHandlers(ItemStack.OnSerialize, ItemStack.OnDeserialize);
    }

    public Item GetItemById(string id)
    {
        return ItemDefinitions.First(item => item.Id == id);
    }

    #endregion

    #region Lifecycle Methods

    void Start()
    {
        InitItemAndInventory();
        InitItemAndInventoryNetworking();
    }

    void Update()
    {
        
    }

    #endregion
}

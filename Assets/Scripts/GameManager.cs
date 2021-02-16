using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameManager contains global information about game state, as well as helper methods.
/// TODO: Perhaps seperating some functionality into different classes.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


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

    #region Constants

    public static readonly string NULL_ITEM_ID = "null";

    #endregion

    #region Variables

    public readonly IList<Item> ItemDefinitions = new List<Item>();

    #endregion

    #region UI

    public InventoryUI InventoryUI;
    public HotbarInventoryUI HotbarUI;
    public Text InteractionText;

    #endregion

    #region Item and Inventory

    void InitItemAndInventory()
    {
        var ItemSprites = Resources.LoadAll<Sprite>("Sprites/items");

        ItemDefinitions.Add(new Item(
            "null",
            "Null",
            "Used to represent empty space.",
            0,
            ItemSprites.Single(sprite => sprite.name == "null")
            ));
        ItemDefinitions.Add(new Item(
            "knife", 
            "Hunting Knife", 
            "A sharp blade used for hunting and utility.", 
            1,
            ItemSprites.Single(sprite => sprite.name == "knife")
            ));
        ItemDefinitions.Add(new HealingItem(
            "bandage", 
            "Bandage", 
            "Bandages will give a small HP revovery.", 
            20,
            ItemSprites.Single(sprite => sprite.name == "bandage")
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

    #region Local Player Details

    public GameObject GetLocalPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public CharacterInventoryAndInteraction GetLocalPlayerInventory()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        return player.GetComponent<CharacterInventoryAndInteraction>();
    }

    #endregion

    #region GameManager Lifecycle Methods

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

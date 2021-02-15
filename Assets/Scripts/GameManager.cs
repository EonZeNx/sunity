using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public readonly IDictionary<string, Item> ItemDefinitions = new Dictionary<string, Item>();

    #endregion

    #region Item and Inventory

    void InitItemAndInventory()
    {
        ItemDefinitions.Add("null", new Item("Null", "Used to represent empty space.", 0));
        ItemDefinitions.Add("knife", new Item("Hunting Knife", "A sharp blade used for hunting and utility.", 1));
        ItemDefinitions.Add("bandage", new Item("Bandage", "Bandages will give a small HP revovery.", 20));
    }

    void InitItemAndInventoryNetworking()
    {
        SerializationManager.RegisterSerializationHandlers(Inventory.OnSerialize, Inventory.OnDeserialize);
        SerializationManager.RegisterSerializationHandlers(ItemStack.OnSerialize, ItemStack.OnDeserialize);
    }

    #endregion

    public GameObject GetLocalPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

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

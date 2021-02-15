using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

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

    [Header("Inventory State")]
    public bool MainInventoryOpen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Open or close inventory based on MainInventoryOpen
        if (MainInventoryOpen && !InventoryPanel.activeSelf)
        {
            InventoryPanel.SetActive(true);
        }
        else if (!MainInventoryOpen && InventoryPanel.activeSelf)
        {
            InventoryPanel.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        MainInventoryOpen = !MainInventoryOpen;
    }
}

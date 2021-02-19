using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    private static PlayerManager _instance;

    public static PlayerManager Instance { get { return _instance; } }

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

    #region Local Player Details

    public GameObject LocalPlayer
    {
        get
        {
            return GameObject.FindGameObjectWithTag("Player");
        }
    }

    public CharacterInventoryAndInteraction LocalPlayerInventory
    {
        get
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            return player.GetComponent<CharacterInventoryAndInteraction>();
        }
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
        
    }

    #endregion
}

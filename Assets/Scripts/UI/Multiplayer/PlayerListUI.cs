using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerNamePrefab;

    private List<GameObject> listOfNames;

    // Start is called before the first frame update
    void Start()
    {
        listOfNames = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        var numberOfClients = PlayerManager.Singleton.PlayerList.Count;
        if(listOfNames.Count != numberOfClients)
        {
            UpdatePlayerList();
            Debug.Log($"New player list for {listOfNames.Count} players");
        }
    }

    /// <summary>
    /// Refresh the player list with players currently on the server
    /// </summary>
    public void UpdatePlayerList()
    {
        // Destroy list of players
        foreach (var name in listOfNames)
        {
            Destroy(name);
        }
        listOfNames = new List<GameObject>();

        // Instantiate new list of players
        var connectedClients = PlayerManager.Singleton.PlayerList;
        foreach(var playerObject in connectedClients)
        {
            // Instantiate name and parent it
            var playerName = Instantiate(playerNamePrefab, transform);
            playerName.GetComponent<PlayerNameUI>().playerNameText.text = $"Player {playerObject.OwnerClientId}";
            listOfNames.Add(playerName);
        }
    }
}

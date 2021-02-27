using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerNamePrefab;

    private List<PlayerNameUI> listOfNames;

    // Start is called before the first frame update
    void Start()
    {
        listOfNames = new List<PlayerNameUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var numberOfClients = NetworkingManager.Singleton.ConnectedClientsList.Count;
        if(listOfNames.Count != numberOfClients)
        {
            UpdatePlayerList();
        }
    }

    /// <summary>
    /// Refresh the player list with players currently on the server
    /// </summary>
    public void UpdatePlayerList()
    {
        // Destroy list of players
        foreach(var name in listOfNames)
        {
            Destroy(name);
        }

        // Instantiate new list of players
        var connectedClients = NetworkingManager.Singleton.ConnectedClientsList;
        foreach(var connectionClient in connectedClients)
        {
            var playerObject = connectionClient.PlayerObject;

            // Instantiate name and parent it
            var playerName = Instantiate(playerNamePrefab, transform).GetComponent<PlayerNameUI>();
            playerName.playerNameText.text = $"Player {playerObject.OwnerClientId}";
            listOfNames.Add(playerName);
        }
    }
}

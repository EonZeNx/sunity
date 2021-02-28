using MLAPI;
using MLAPI.Messaging;
using System.Text;
using UnityEngine;

public class DeveloperUI : MonoBehaviour
{
    public void LogAllInventories()
    {
        var connectedClients = PlayerManager.Singleton.PlayerList;
        foreach (var client in connectedClients)
        {
            var debugStringBuilder = new StringBuilder();

            debugStringBuilder.AppendLine($"Player {client.OwnerClientId}'s inventory:");
            
            var clientInventory = client.GetComponent<EntityInventory>();

            debugStringBuilder.AppendLine("Main Inventory:");
            debugStringBuilder.AppendLine(clientInventory.GetMainInventory().ToString());
            debugStringBuilder.AppendLine("Hotbar:");
            debugStringBuilder.AppendLine(clientInventory.GetHotbarInventory().ToString());
            debugStringBuilder.AppendLine("Mouse Slot:");
            debugStringBuilder.AppendLine(clientInventory.GetMouseSlot().ToString());

            Debug.Log(debugStringBuilder.ToString());
        }
    }

    public void GiveEveryoneBandages()
    {
        var connectedClients = PlayerManager.Singleton.PlayerList;
        foreach (var client in connectedClients)
        {
            client.GetComponent<EntityInventory>().GiveBandages();
        }
    }
}

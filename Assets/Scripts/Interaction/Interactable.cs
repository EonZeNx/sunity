using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : NetworkedBehaviour
{
    public string DisplayName;
    
    public virtual void Interact(CharacterInventoryAndInteraction characterInventory)
    {
        return;
    }
}

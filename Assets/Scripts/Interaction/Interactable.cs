using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string DisplayName;
    
    public virtual void Interact(CharacterInventory characterInventory)
    {
        return;
    }
}

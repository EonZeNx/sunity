using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public string ItemId;
    public int Quantity;

    public ItemInteractable(): base() { }

    [ServerRPC(RequireOwnership = false)]
    public override void Interact(EntityInteraction entityInteraction)
    {
        var playerInventory = entityInteraction.GetComponent<EntityInventory>();
        if(playerInventory == null)
        {
            return;
        }

        var overflow = playerInventory.PickupItemStack(new ItemStack(ItemId, Quantity));
        if (overflow.IsEmpty())
        {
            Destroy(gameObject);
            NetworkedObject.UnSpawn();
        } else
        {
            ItemId = overflow.ItemId;
            Quantity = overflow.Quantity;
        }
    }
}

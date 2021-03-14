using UnityEngine;

/// <summary>
/// HealingItem is an item, which when used heals the player and consumes itself.
/// </summary>
public class HealingItem : Item
{
    public HealingItem(string Id, string Name, string Description, int MaxStackSize, Sprite Sprite, GameObject InteractablePrefab) : base(Id, Name, Description, MaxStackSize, Sprite, InteractablePrefab)
    { }
    
    /// <summary>
    /// Use the one from the stack, and heal the player.
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public override ItemStack OnUseSecondary(ItemStack stack, EntityInventory character)
    {
        var resultStack = new ItemStack(stack.ItemId, stack.Quantity);

        resultStack.Quantity -= 1;
        resultStack.UpdateEmptyStack();
        Debug.Log("Bandage consumed.");

        return resultStack;
    }
}

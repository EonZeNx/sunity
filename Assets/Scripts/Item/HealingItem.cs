using UnityEngine;

public class HealingItem : Item
{
    public HealingItem(string Id, string Name, string Description, int MaxStackSize, Sprite Sprite) : base(Id, Name, Description, MaxStackSize, Sprite)
    { }
    
    public override ItemStack OnUseSecondary(ItemStack stack, CharacterInventory character)
    {
        var resultStack = new ItemStack(stack.ItemId, stack.Quantity);

        resultStack.Quantity -= 1;
        resultStack.UpdateEmptyStack();
        Debug.Log("Bandage consumed.");

        return resultStack;
    }
}

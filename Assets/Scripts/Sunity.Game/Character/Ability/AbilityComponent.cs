
using Sunity.Game.Character.Movement;
using UnityEngine;

namespace Sunity.Game
{
    public enum EAbilities
    {
        Grapple,
        Boost,
        Energize,
        Rage,
        Skip,
        Shift,
        Tether
    }
    
    public abstract class AbilityComponent : MonoBehaviour
    {
        public bool canUseAbility;
        
        // Maximum number of uses
        protected int baseCharges;
        public int BaseCharges => baseCharges;
        
        // Number of uses
        protected int charges;
        public int Charges => charges;
        public int UsedCharges => baseCharges - charges;
        
        // Maximum single charge cooldown
        protected float baseCooldown;
        public float BaseCooldown => baseCooldown;
        
        // Single charge cooldown
        protected float cooldown;
        public float Cooldown => cooldown;
        
        // Single charge cooldown
        protected bool usingAbility;
        public bool UsingAbility => usingAbility;

        public abstract void UseAbility();
        public abstract void EndAbility();
        
        public abstract void UseCharge();
        public abstract void GiveCharge();
        
        public abstract void StartCooldown();
        public abstract void ResetCooldown();
        public abstract void CooldownClock();

        public abstract void LateConstructor(AdvMovement advMove);
    }
}

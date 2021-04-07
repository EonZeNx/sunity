using System;
using System.Collections;
using System.Collections.Generic;
using Sunity.Game.Character.Movement;
using Sunity.Game.Utils;
using UnityEngine;

namespace Sunity.Game
{
    public class BoostComponent : AbilityComponent
    {
        #region Variables

        #region References
        
        private AdvMovement advMovement;

        #endregion

        #region Boost properties

        [Header("Boost properties")]
        [SerializeField]
        private float boostSpeed;

        #endregion

        #endregion
        
        public override void LateConstructor(AdvMovement advMove)
        {
            advMovement = advMove;
            
            // Shared properties
            canUseAbility = true;
            
            baseCharges = 3;
            baseCooldown = 6f;
            usingAbility = false;
            
            // Boost properties
            boostSpeed = 7f;
        }

        public override void UseAbility()
        {
            if (!canUseAbility || usingAbility) return;
            
            usingAbility = true;
            UseCharge();

            // TODO: Add to forces queue to prevent race condition.
            Vector3 velocityDirection = advMovement.XYZForce3.normalized;
            Vector3 boostVelocity = velocityDirection * boostSpeed;
            advMovement.XYZForce3 += boostVelocity;
            
            EndAbility();
        }

        public override void EndAbility()
        {
            if (!canUseAbility || !usingAbility) return;
            
            usingAbility = false;
        }
        
        public override void UseCharge()
        {
            charges -= 1;
        }
        
        public override void GiveCharge()
        {
            charges += 1;
        }
        
        public override void StartCooldown()
        {
            cooldown = baseCooldown;
        }
        
        public override void ResetCooldown()
        {
            cooldown = 0f;
            charges = baseCharges;
        }

        public override void CooldownClock()
        {
            // Cooldown
            if (UsedCharges > 0)
            {
                cooldown -= Time.deltaTime;
                if (cooldown < MathUtils.FLOAT_ZERO)
                {
                    GiveCharge();
                    if (UsedCharges == 0) cooldown = 0f;
                    else StartCooldown();
                }
            }
        }

        void Update()
        {
            CooldownClock();
        }
    }
}

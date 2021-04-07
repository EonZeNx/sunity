using System;
using System.Collections;
using System.Collections.Generic;
using Sunity.Game.Character.Movement;
using Sunity.Game.Utils;
using UnityEngine;

namespace Sunity.Game
{
    public class GrappleComponent : AbilityComponent
    {
        #region Variables

        #region References
        
        private Transform aimTransform;
        private Vector3 grappleStart;
        public Vector3 worldGrappleStart => transform.TransformPoint(grappleStart);
        private Vector3 grappleEnd;
        private AdvMovement advMovement;

        #endregion

        #region Grapple properties

        [Header("Grapple properties")]
        [SerializeField]
        private float range;
        [SerializeField]
        private float directAcceleration;
        [SerializeField]
        private float aimVelocity;
        [SerializeField]
        private float detachAngle;

        #endregion

        #region Grapple variables

        private bool isShootingGrapple;

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
            
            // Grapple properties
            range = 10f;
            directAcceleration = 5f;
            aimVelocity = 5f;
            detachAngle = 10f;
        }

        public override void UseAbility()
        {
            if (!canUseAbility) return;
            
            usingAbility = true;
            isShootingGrapple = true;

            RaycastHit hit;
            bool didHit = Physics.Raycast(worldGrappleStart, aimTransform.forward, out hit, range);
            if (!didHit) return;

            grappleEnd = hit.point;
            
            UseCharge();
        }

        public override void EndAbility()
        {
            if (!canUseAbility) return;
            
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
                    if (UsedCharges == 0)
                    {
                        cooldown = 0f;
                    }
                    else
                    {
                        StartCooldown();
                    }
                }
            }
        }

        void Update()
        {
            CooldownClock();
            
            // Not grappling
            if (!usingAbility) return;

            if (isShootingGrapple)
            {
                
            }

            // Angle between aim and direction to grapple point exceeds detach angle 
            if (Vector3.Angle(aimTransform.forward, grappleEnd - grappleStart) > detachAngle)
            {
                EndAbility();
            }
        }
    }
}

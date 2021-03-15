using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveData
{
    #region Structs

    [System.Serializable]
    public struct FallingMoveStruct
    {
        // Acceleration of movement while falling.
        public float acceleration;
        
        // Maximum movement speed.
        public float maxSpeed;

        public FallingMoveStruct(float acceleration, float maxSpeed)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
        }
    }

    [System.Serializable]
    public struct GroundedMoveStruct
    {
        // Acceleration of character while on ground.
        public float acceleration;

        // Braking friction while on ground.
        public float friction;

        // Maximum speed possible while on ground.
        public float maxSpeed;

        public GroundedMoveStruct(float acceleration, float friction, float maxSpeed)
        {
            this.acceleration = acceleration;
            this.friction = friction;
            this.maxSpeed = maxSpeed;
        }
    }
    
    [System.Serializable]
    public struct JumpStruct
    {
        // Jump force to launch character at.
        public float force;

        // Maximum number of jumps.
        public int maxJumps;

        public JumpStruct(float force = 9.81f, int maxJumps = 2)
        {
            this.force = force;
            this.maxJumps = maxJumps;
        }
    }
    
    [System.Serializable]
    public struct GravityStruct
    {
        // The up direction for the character.
        public Vector3 worldUp;

        // Gravity force opposing world up. Almost always a negative value.
        public float gravity;

        // Gravity to apply when beginning to fall.
        // Consider a much lower value than gravity to keep a good game feel.
        public float fallGravity;

        public GravityStruct(Vector3 worldUp, float gravity = -9.81f, float fallGravity = -3.3f)
        {
            this.worldUp = worldUp;
            this.gravity = gravity;
            this.fallGravity = fallGravity;
        }
    }

    [System.Serializable]
    public struct SlideMoveStruct
    {
        // Amount to boost the player by when starting to slide
        public float boostSpeed;
        
        // Minimum speed to slide before transitioning out of slide
        public float minSpeed;

        public SlideMoveStruct(float boostSpeed, float minSpeed)
        {
            this.boostSpeed = boostSpeed;
            this.minSpeed = minSpeed;
        }
    }

    #endregion

    #region Enums

    [System.Serializable]
    public enum GMoveEnum
    {
        Jogging,
        Sprinting,
        Crouching,
        Sliding
    }

    #endregion
}

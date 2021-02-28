using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdvancedMovement
{
    #region Structs

    [System.Serializable]
    public struct GravityStruct
    {
        // The up direction for the character.
        public Vector3 worldUp;

        // Gravity force opposing world up. Almost always a negative value.
        public float gravity;

        public GravityStruct(Vector3 worldUp, float gravity = -9.81f)
        {
            this.worldUp = worldUp;
            this.gravity = gravity;
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
    public struct GMoveStruct
    {
        // Speed at which character can accelerate.
        // Comparable to grip.
        public float friction;
        
        // Acceleration of character under this movement struct.
        public float acceleration;

        // Maximum speed before applying braking forces.
        public float maxSpeed;
        
        // Speed at which character will decelerate.
        // Comparable to grip while braking.
        public float brakingFriction;
        
        // Deceleration of character while braking under this movement struct.
        public float brakingDeceleration;

        public GMoveStruct(float friction = 8f, float acceleration = 2048f, float maxSpeed = 50f, float brakingFriction = 8f, float brakingDeceleration = 2048f)
        {
            this.friction = friction;
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.brakingFriction = brakingFriction;
            this.brakingDeceleration = brakingDeceleration;
        }
    }

    #endregion

    #region Enums

    [System.Serializable]
    public enum EGait
    {
        Jogging,
        Sprinting
    }

    #endregion
}

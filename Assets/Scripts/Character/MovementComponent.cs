using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Character
{
    public class MovementComponent : NetworkedBehaviour
    {
        #region Structs
        [System.Serializable]
        public struct GlobalMovementStruct
        {
            // Gravity force per delta time
            public float gravity;

            // Reverse of gravity direction. This ensures consistence when dealing with other aspects of Unity
            public Vector3 entityUp;
            
            // Maximum speed when launching or falling
            public float terminalVelocity;

            // Whether or not to rotate the character to the current gravity
            public bool rotEntityToEntityUp;

            public GlobalMovementStruct(Vector3 entityUp, float gravity = -9.81f, float terminalVelocity = 150f,  
                bool rotEntityToEntityUp = false)
            {
                this.gravity = gravity;
                this.entityUp = entityUp;
                this.terminalVelocity = terminalVelocity;
                this.rotEntityToEntityUp = rotEntityToEntityUp;
            }
        }
        
        [System.Serializable]
        public struct BasicMovementStruct
        {
            // How much control when changing directions
            public float groundFriction;

            // Maximum speed while moving
            public float maxSpeed;

            // Maximum acceleration while moving
            public float acceleration;

            // Maximum deceleration while moving
            public float brakingDeceleration;

            // Friction applied when acceleration == 0 or exceeding max speed
            public float brakingFriction;

            // Optional friction factor when calculating actual friction
            public float brakingFrictionFactor;

            public BasicMovementStruct(float groundFriction = 8f, float maxSpeed = 10f, float acceleration = 5f, 
                float brakingDeceleration = 7f, float brakingFriction = 8f, float brakingFrictionFactor = 1f)
            {
                this.groundFriction = groundFriction;
                this.maxSpeed = maxSpeed;
                this.acceleration = acceleration;
                this.brakingDeceleration = brakingDeceleration;
                this.brakingFriction = brakingFriction;
                this.brakingFrictionFactor = brakingFrictionFactor;
            }
        }
        
        [System.Serializable]
        public struct JumpMovementStruct
        {
            // Jump force to apply.
            public float force;
            
            // Maximum number of jumps the entity can perform.
            public int maxNumberOfJumps;

            public JumpMovementStruct(float force = 10f, int maxNumberOfJumps = 2)
            {
                this.force = force;
                this.maxNumberOfJumps = maxNumberOfJumps;
            }
        }
        #endregion
        
        #region Enums
        public enum MovementModeEnum
        {
            Grounded,
            Falling
        }
        public enum GaitEnum
        {
            Walk,
            Run
        }
        public enum StanceEnum
        {
            Crouched,
            Standing
        }
        #endregion

        #region Variables
        [Header("External References")]
        public CharacterController controller;
        public Transform aimLocation;
        public Transform groundCheck;

        [Header("Movement Structs")]
        public GlobalMovementStruct globalSettings;
        public BasicMovementStruct basicSettings;
        public JumpMovementStruct jumpSettings;

        [Header("Movement Enums")]
        public MovementModeEnum movementMode;

        [Header("Inputs")]
        public Vector3 lastMoveInput;

        [Header("Jump")]
        public int currentJumps;
        public bool pendingJump;

        [Header("Forces")]
        public Vector3 oldVelocity;
        public Vector3 velocity;
        public Vector3 oldAcceleration;
        public Vector3 acceleration;
        
        [Header("Ground Check")]
        public float groundCheckRadius;
        public LayerMask groundCollisionMask;

        public bool isGrounded
        {
            get
            {
                return movementMode == MovementModeEnum.Grounded;
            }
        }
        public bool isFalling
        {
            get
            {
                return movementMode == MovementModeEnum.Falling;
            }
        }

        #region Readonly
        private float _vertForces;
        private Vector3 _horiForces;
        #endregion
        
        #endregion

        // Constructor
        public MovementComponent()
        {
            // Movement Structs
            globalSettings = new GlobalMovementStruct(Vector3.up);
            basicSettings = new BasicMovementStruct();
            jumpSettings = new JumpMovementStruct(maxNumberOfJumps:-1);

            // Movement Enums
            movementMode = MovementModeEnum.Falling;

            // Jump
            // currentJumps = JumpMovement.MaxNumberOfJumps;
            currentJumps = -1;
            pendingJump = false;
            
            // Forces
            oldVelocity = Vector3.zero;
            velocity = Vector3.zero;
            oldAcceleration = Vector3.zero;
            acceleration = Vector3.zero;
            
            // Ground Check
            groundCheckRadius = 0.4f;
            
            // Read only
            _vertForces = 0f;
            _horiForces = Vector3.zero;
        }
        
        #region Utils
        
        /// <summary>
        /// Keep track of current and old values.
        /// May be useful for later such as different animations due to changes in acceleration.
        /// </summary>
        private void UpdateReadForces()
        {
            oldVelocity = velocity;
            velocity = controller.velocity;
            
            oldAcceleration = acceleration;
            acceleration = Vector3.zero;
        }
        
        /// <summary>
        /// Check if the entity is on the ground. Broadcasts a message if isGrounded is updated.
        /// </summary>
        private void GroundCheck()
        {
            bool newIsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundCollisionMask);
            
            // If new state is different, Broadcast a message.
            if (newIsGrounded != isGrounded)
            {
                if (newIsGrounded)
                {
                    BroadcastMessage("OnLanded");
                }
                else {
                    BroadcastMessage("OnFalling");
                }
            }
        }
        
        #endregion

        #region Messages
        
        private void OnLanded()
        {
            movementMode = MovementModeEnum.Grounded;
            _vertForces = globalSettings.gravity;
            if (currentJumps != -1)
            {
                currentJumps = jumpSettings.maxNumberOfJumps;
            }
        }
        private void OnFalling()
        {
            movementMode = MovementModeEnum.Falling;
        }
        private void OnJumpInput()
        {
            if (currentJumps > 0)
            {
                pendingJump = true;
                currentJumps -= 1;
            }

            if (currentJumps == -1 && jumpSettings.maxNumberOfJumps == -1)
            {
                pendingJump = true;
            }
            else
            {
                currentJumps = 0;
                jumpSettings = new JumpMovementStruct();
            }
        }
        private void OnMoveInput(Vector3 moveInput)
        {
            lastMoveInput = moveInput;
        }
        
        #endregion

        #region Movement Calculations

        #region Grounded HForces

        /// <summary>
        /// Calculates the grounded braking horizontal forces for the entity.
        /// </summary>
        /// <param name="isExceedingMaxSpeed">Whether or not the entity was exceeding their maximum speed.</param>
        private void CalcGroundBrakingHForces(bool isExceedingMaxSpeed)
        {
            // Calculate the braking friction.
            Vector3 brakingDirection = -_horiForces.normalized;
            Vector3 newHoriForces = _horiForces + ((-basicSettings.brakingFriction) * _horiForces + brakingDirection) * Time.deltaTime;

            // If was above max speed and braking forced them lower than max speed, adjust.
            if (isExceedingMaxSpeed && newHoriForces.sqrMagnitude > basicSettings.maxSpeed * basicSettings.maxSpeed)
            {
                newHoriForces = newHoriForces.normalized * basicSettings.maxSpeed;
            }

            // If braking forced a reverse change in direction, set to zero.
            if (Vector3.Dot(newHoriForces, _horiForces) < 0)
            {
                newHoriForces = Vector3.zero;
            }
            _horiForces = newHoriForces;
        }

        /// <summary>
        /// Lateral movement input for grounded entities.
        /// </summary>
        private void CalcGroundedInputForces()
        {
            // Lateral movement calc
            // x then z due to Unity's different coord layout
            float rotTargetAngle = Mathf.Atan2(lastMoveInput.x, lastMoveInput.z) * Mathf.Rad2Deg + aimLocation.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, rotTargetAngle, 0f) * Vector3.forward;

            // Direction change consideration
            float velocitySize = (_horiForces + moveDirection).magnitude;
            Vector3 inputDirection = _horiForces - ((_horiForces - moveDirection * velocitySize) *
                                                    Mathf.Min(Time.deltaTime * basicSettings.groundFriction, 1f));
            Vector3 inputAcceleration = inputDirection * basicSettings.acceleration;

            // Check if new horizontal acceleration is exceeding the speed limit and adjust if so.
            Vector3 newHoriForces = inputAcceleration;
            if (newHoriForces.sqrMagnitude > basicSettings.maxSpeed * basicSettings.maxSpeed)
            {
                newHoriForces = newHoriForces.normalized * basicSettings.maxSpeed;
            }
            _horiForces = newHoriForces;
        }

        /// <summary>
        /// Calculate horizontal forces for the entity while on the ground.
        /// </summary>
        private void CalcGroundedHForces()
        {
            // Brake/resist current movement if either exceeding maximum speed, have no acceleration, or input is too small to register.
            bool isExceedingMaxSpeed = _horiForces.sqrMagnitude > basicSettings.maxSpeed * basicSettings.maxSpeed;
            bool zeroAcceleration = lastMoveInput.sqrMagnitude < 0.1f;
            
            // Only decelerate if exceeding max speed or have no input.
            if (isExceedingMaxSpeed || zeroAcceleration)
            {
                CalcGroundBrakingHForces(isExceedingMaxSpeed);
            }

            if (!zeroAcceleration)
            {
                CalcGroundedInputForces();
            }
        }
        
        #endregion

        #region Falling HForces

        /// <summary>
        /// Calculate the horizontal forces for the entity while falling.
        /// </summary>
        private void CalcFallingHForces()
        {
            // TODO: Figure CalcFallingHForces out.
            return;
        }
        #endregion
        
        /// <summary>
        /// Calls the appropriate CalcHForces function for the current movement state.
        /// </summary>
        private void CalcHorizontalForces()
        {
            switch (movementMode)
            {
                case MovementModeEnum.Grounded:
                    CalcGroundedHForces();
                    break;
                case MovementModeEnum.Falling:
                    CalcFallingHForces();
                    break;
                default:
                    CalcGroundedHForces();
                    break;
            }
        }
        
        /// <summary>
        /// Calculates the vertical forces applied to this entity.
        /// </summary>
        private void CalcVerticalForces()
        {
            if (!isGrounded)
            {
                _vertForces += globalSettings.gravity * Time.deltaTime;
            }
            
            // If pending a jump, override the vertical force to give a proper jump.
            if (pendingJump)
            {
                _vertForces = jumpSettings.force;
                pendingJump = false;
            }
        }

        /// <summary>
        /// Calls all other movement calculation functions then finally applies them.
        /// </summary>
        private void CalcMovement()
        {
            CalcHorizontalForces();
            CalcVerticalForces();

            acceleration = _horiForces;
            acceleration += globalSettings.entityUp * _vertForces;
            controller.Move(acceleration * Time.deltaTime);
        }

        #endregion

        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update()
        {
            if (!IsLocalPlayer)
            {
                return;
            }

            GroundCheck();
            CalcMovement();

            UpdateReadForces();
        }
    }
}


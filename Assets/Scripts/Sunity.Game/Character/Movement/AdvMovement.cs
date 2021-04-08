using Sunity.Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sunity.Game.Character.Movement
{
    public class AdvMovement : MonoBehaviour
    {
        #region Variables

        #region References

        [Header("References")]
        [SerializeField]
        private CharacterController controller;
        [SerializeField]
        private Transform aimTransform;
        private AbilityComponent _abilityMoveComponent;

        #endregion

        #region Camera

        private float _cameraPitch = 0f;

        #endregion

        #region Input

        [Header("Gameplay")]
        public bool toggleSprint = true;
        public bool toggleCrouch = true;
        [Header("Gameplay - Sensitivity")]
        public float xSensitivity;
        public float ySensitivity;

        private bool _hasMoveInput;
        private Vector3 _moveInput;

        private bool _isHoldingJump;

        #endregion

        #region Movement

        private bool _isGrounded;
        private bool _jumpedThisFrame;

        private int _cJumps;
        public bool pendingJump;

        #region Bunnyhop variables

        private float _bHopTimeBuffer = -1f;
        private float _bHopTimeWindow = 0.05f;
        private Vector3 _bHopVelocity;

        #endregion

        #region Movement Config

        [Header("Movement Config")]
        public GravityStruct defaultGStruct = new GravityStruct(Vector3.up, -9.81f);
        public JumpStruct defaultJStruct = new JumpStruct(9.81f, 1);

        public GroundedMoveStruct joggingMoveStruct = new GroundedMoveStruct(10f, 4f, 8f);
        public GroundedMoveStruct sprintingMoveStruct = new GroundedMoveStruct(20f, 4f, 14f);
        public GroundedMoveStruct crouchingMoveStruct = new GroundedMoveStruct(10f, 4f, 5f);
        public GroundedMoveStruct slidingMoveStruct = new GroundedMoveStruct(0f, 0.66f, 0f);

        public FallingMoveStruct defaultFMoveStruct = new FallingMoveStruct(8f, 1f);

        public SlideMoveStruct defaultSlideMoveStruct = new SlideMoveStruct(8f, 4f);

        #endregion

        #region Current Movement

        private GravityStruct _cGStruct;
        private JumpStruct _cJStruct;

        private GroundedMoveStruct _cGMoveStruct;
        private FallingMoveStruct _cFMoveStruct;

        private GMoveEnum _cGMoveEnum;

        private SlideMoveStruct _cSlideMoveStruct;

        #endregion

        #region Movement ability variables

        [Header("Movement ability variables")]
        [SerializeField]
        private EAbilities abilityType = EAbilities.Boost;

        #endregion

        #endregion

        #region Forces

        private Vector2 _xzForce;
        private float _yForce;

        public float XZSpeed => _xzForce.magnitude;
        public float YSpeed => _yForce;
        public Vector3 XZForce3
        {
            get { return new Vector3(_xzForce.x, 0f, _xzForce.y); }
            set { _xzForce = new Vector2(value.x, value.z); }
        }
        public Vector3 XYZForce3
        {
            get { return new Vector3(_xzForce.x, _yForce, _xzForce.y); }
            set { _xzForce = new Vector2(value.x, value.z); _yForce = value.y; }
        }

        private List<Vector3> pendingLaunches = new List<Vector3>();

        #endregion

        #endregion


        #region Input Messages

        public void OnMove(InputValue value)
        {
            Vector2 inputVec = value.Get<Vector2>();
            _hasMoveInput = Mathf.Abs(inputVec.x) > 0.05f || Mathf.Abs(inputVec.y) > 0.05f;
            _moveInput = new Vector3(inputVec.x, 0f, inputVec.y).normalized;
        }

        public void OnLook(InputValue value)
        {
            Vector2 inputVec = value.Get<Vector2>();
            float xInput = inputVec.x * 0.075f * xSensitivity;
            float yInput = inputVec.y * 0.075f * ySensitivity;

            // Use absolute values to clamp pitch to +-90 degrees.
            _cameraPitch -= yInput;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);

            aimTransform.localEulerAngles = Vector3.right * _cameraPitch;
            transform.Rotate(Vector3.up * xInput);
        }

        public void OnJump(InputValue value)
        {
            float jumpValue = value.Get<float>();
            WantsToJump(jumpValue > 0.5f);
        }

        private void WantsToJump(bool pressedJump)
        {
            _isHoldingJump = pressedJump;

            if (!pressedJump) return;

            if (_cJumps > 0 || _cJStruct.maxJumps == -1)
            {
                if (_cJStruct.maxJumps != -1) _cJumps -= 1;
                pendingJump = true;
            }
        }

        public void OnSprint(InputValue value)
        {
            float sprintValue = value.Get<float>();
            bool sprintPressed = sprintValue > 0.5f;

            if (toggleSprint)
            {
                if (sprintPressed)
                {
                    GMoveEnum newValue = _cGMoveEnum == GMoveEnum.Sprinting ? GMoveEnum.Jogging : GMoveEnum.Sprinting;
                    UpdateCGMoveStruct(newValue);
                }
            }
            else
            {
                GMoveEnum newValue = sprintPressed ? GMoveEnum.Sprinting : GMoveEnum.Jogging;
                UpdateCGMoveStruct(newValue);
            }
        }

        private void UpdateCGMoveStruct(GMoveEnum newValue)
        {
            if (_cGMoveEnum == newValue) return;

            _cGMoveEnum = newValue;
            switch (newValue)
            {
                case GMoveEnum.Jogging:
                    _cGMoveStruct = joggingMoveStruct;
                    break;
                case GMoveEnum.Sprinting:
                    _cGMoveStruct = sprintingMoveStruct;
                    break;
                case GMoveEnum.Crouching:
                    _cGMoveStruct = crouchingMoveStruct;
                    break;
                case GMoveEnum.Sliding:
                    _cGMoveStruct = slidingMoveStruct;
                    break;
                default:
                    _cGMoveStruct = sprintingMoveStruct;
                    break;
            }
        }

        private void OnCrouch(InputValue value)
        {
            float crouchValue = value.Get<float>();
            bool crouchPressed = crouchValue > 0.5f;

            if (toggleCrouch)
            {
                if (crouchPressed)
                {
                    if (_cGMoveEnum == GMoveEnum.Sliding) EndSlide();
                    else if (_cGMoveEnum == GMoveEnum.Sprinting) StartSlide();
                    else
                    {
                        if (_cGMoveEnum == GMoveEnum.Crouching) EndCrouch();
                        else StartCrouch();
                    }
                }
            }
            else
            {
                if (_cGMoveEnum == GMoveEnum.Sliding) EndSlide();
                else if (_cGMoveEnum == GMoveEnum.Sprinting) StartSlide();
                else
                {
                    if (_cGMoveEnum == GMoveEnum.Crouching) EndCrouch();
                    else StartCrouch();
                }
            }
        }

        private void OnMovementAbility(InputValue value)
        {
            float moveAbilityValue = value.Get<float>();
            bool pressed = moveAbilityValue > 0.5f;
            
            if (pressed)
            {
                _abilityMoveComponent.UseAbility();
            }
            else
            {
                _abilityMoveComponent.EndAbility();
            }
        }

        #endregion

        #region Messages

        private void OnLanded()
        {
            _cJumps = _cJStruct.maxJumps;

            _bHopVelocity = XZForce3;
            _bHopTimeBuffer = 0f;

            if (_isHoldingJump)
            {
                WantsToJump(true);
            }
        }

        private void OnLeaveGround()
        {
            if (_jumpedThisFrame) return;

            // TODO: Make this depend on if the entity is standing on a moving object.
            // If began falling, apply a lower gravity to prevent sudden fast falls.
            _yForce = _cGStruct.fallGravity;
        }

        #endregion

        #region Movement

        #region Shared movement

        private void GetWishValues(out Vector3 wishVel, out Vector3 wishDir, out float wishSpeed, float acceleration)
        {
            wishVel = Vector3.zero;
            wishDir = Vector3.zero;
            wishSpeed = 0f;

            Vector3 forward = aimTransform.forward;
            Vector3 right = aimTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Unity forward is Z+
            wishVel = forward * (acceleration * _moveInput.z)
                      + right * (acceleration * _moveInput.x);
            wishVel.y = 0f;

            wishSpeed = wishVel.magnitude;
            wishDir = wishVel.normalized;
        }

        private Vector3 Accelerate(Vector3 currentVelocity, Vector3 wishDir, float wishSpeed, float accel, float surfaceFriction)
        {
            // See if we are changing direction a bit
            float currentSpeed = Vector3.Dot(currentVelocity, wishDir);

            // Reduce wishSpeed by the amount of veer.
            float addSpeed = Mathf.Max(wishSpeed - currentSpeed, 0f);

            // If not going to add any speed, done.
            if (addSpeed < MathUtils.FLOAT_ZERO) return Vector3.zero;

            // Determine amount of acceleration.
            float accelSpeed = accel * Time.deltaTime * wishSpeed * surfaceFriction;

            // Cap addSpeed
            accelSpeed = Mathf.Min(accelSpeed, addSpeed);

            return accelSpeed * wishDir;
        }

        private Vector3 Friction(Vector3 velocity, float stopSpeed, float friction)
        {
            float speed = velocity.magnitude;
            if (speed < 0.0001905f) return velocity;

            // apply ground friction
            float control = Mathf.Max(speed, stopSpeed);
            float drop = control * friction * Time.deltaTime;

            // scale the velocity
            float newSpeed = speed - drop;
            if (newSpeed < 0f) newSpeed = 0f;

            if (!MathUtils.FloatIsEqual(newSpeed, speed))
            {
                newSpeed /= speed;
                return velocity * newSpeed;
            }

            return velocity;
        }

        #endregion

        #region InAir movement

        private void InAirMovement()
        {
            Vector3 wishVel, wishDir;
            float wishSpeed;
            GetWishValues(out wishVel, out wishDir, out wishSpeed, _cFMoveStruct.acceleration);

            Vector3 accelResult = Accelerate(XZForce3, wishDir, wishSpeed, _cFMoveStruct.acceleration, 1f);
            Vector3 newXZForce = XZForce3 + accelResult;

            _xzForce = new Vector2(newXZForce.x, newXZForce.z);
        }

        #endregion

        #region Grounded movement

        private void StartCrouch()
        {
            UpdateCGMoveStruct(GMoveEnum.Crouching);
        }

        private void EndCrouch()
        {
            UpdateCGMoveStruct(GMoveEnum.Jogging);
        }

        private void StartSlide()
        {
            UpdateCGMoveStruct(GMoveEnum.Sliding);

            Vector3 boosted = XZForce3.normalized * (_cSlideMoveStruct.boostSpeed);
            _xzForce += new Vector2(boosted.x, boosted.z);
        }

        private void EndSlide()
        {
            UpdateCGMoveStruct(GMoveEnum.Sprinting);
        }

        private void GroundedMovement()
        {
            // Wish values
            Vector3 wishVel, wishDir;
            float wishSpeed;
            GetWishValues(out wishVel, out wishDir, out wishSpeed, _cGMoveStruct.acceleration);

            if (!MathUtils.FloatIsEqual(wishSpeed, 0.0f) && wishSpeed > _cGMoveStruct.maxSpeed)
            {
                wishVel *= _cGMoveStruct.maxSpeed / wishSpeed;
                wishSpeed = _cGMoveStruct.maxSpeed;
            }

            Vector3 accelResult = Accelerate(XZForce3, wishDir, wishSpeed, _cGMoveStruct.acceleration, 1f);
            Vector3 newXZForce = XZForce3 + accelResult;
            newXZForce.y = 0f;

            newXZForce = Friction(newXZForce, 0f, _cGMoveStruct.friction);

            _xzForce = new Vector2(newXZForce.x, newXZForce.z);

            // BHop time buffer
            if (_bHopTimeBuffer > -1f)
            {
                if (_bHopTimeBuffer > _bHopTimeWindow)
                {
                    _bHopTimeBuffer = -1f;
                }
                else
                {
                    _bHopTimeBuffer += Time.deltaTime;
                }
            }

            // Check if going too slow to slide
            if (_cGMoveEnum == GMoveEnum.Sliding)
            {
                if (XZSpeed < _cSlideMoveStruct.minSpeed) StartCrouch();
            }
        }

        #endregion

        #region Launch

        public void AddPendingLaunch(Vector3 launch)
        {
            pendingLaunches.Add(launch);
        }

        public void ApplyLaunches()
        {
            if (pendingLaunches.Count > 0)
            {
                foreach (Vector3 launch in pendingLaunches)
                {
                    _xzForce += new Vector2(launch.x, launch.z);
                    _yForce += launch.y;
                }

                pendingLaunches = new List<Vector3>();
            }
        }

        #endregion
        
        #region Movement Ability

        private void UpdateMovementAbility(EAbilities newState)
        {
            // Remove current ability movement component.
            if (abilityType == EAbilities.Grapple) Destroy(GetComponent<GrappleComponent>());
            else if (abilityType == EAbilities.Boost) Destroy(GetComponent<BoostComponent>());
            else throw new NullReferenceException();

            abilityType = newState;
            // Add new ability movement component.
            if (abilityType == EAbilities.Grapple) _abilityMoveComponent = gameObject.AddComponent<GrappleComponent>();
            else if (abilityType == EAbilities.Boost) _abilityMoveComponent = gameObject.AddComponent<BoostComponent>();
            else throw new NullReferenceException();

            _abilityMoveComponent.LateConstructor(this);
        }

        #endregion

        #endregion

        #region Checks

        private void GroundCheck()
        {
            if (_isGrounded == controller.isGrounded) return;

            _isGrounded = controller.isGrounded;
            if (_isGrounded)
            {
                BroadcastMessage("OnLanded");
            }
            else
            {
                BroadcastMessage("OnLeaveGround");
            }
        }

        #endregion

        private void Gravity()
        {
            _yForce = _isGrounded
                ? _cGStruct.gravity
                : _yForce + (_cGStruct.gravity * Time.deltaTime);
        }

        private void Jump()
        {
            if (!pendingJump) return;

            _yForce = _cJStruct.force;
            pendingJump = false;
            _jumpedThisFrame = true;

            if (_bHopTimeBuffer < _bHopTimeWindow && _bHopTimeBuffer > 0f)
            {
                XZForce3 = _bHopVelocity;
                _bHopTimeBuffer = -1f;
            }
        }

        private void Movement()
        {
            Gravity();
            Jump();

            if (_isGrounded)
            {
                GroundedMovement();
            }
            else
            {
                InAirMovement();
            }

            Vector3 moveForce = new Vector3(_xzForce.x, _yForce, _xzForce.y);
            Vector3 deltaMoveForce = moveForce * Time.deltaTime;

            controller.Move(deltaMoveForce);
        }

        #region Overrides

        private void Awake()
        {
            _cGStruct = defaultGStruct;
            _cJStruct = defaultJStruct;

            _cGMoveStruct = joggingMoveStruct;
            _cFMoveStruct = defaultFMoveStruct;

            _cSlideMoveStruct = defaultSlideMoveStruct;
            
            UpdateMovementAbility(abilityType);
        }

        // Update is called once per frame
        void Update()
        {
            Movement();

            GroundCheck();

            ApplyLaunches();
        }

        private void LateUpdate()
        {
            _jumpedThisFrame = false;
        }

        #endregion
    }
}
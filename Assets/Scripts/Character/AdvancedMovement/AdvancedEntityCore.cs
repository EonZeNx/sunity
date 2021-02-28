using System.Collections;
using System.Collections.Generic;
using AdvancedMovement;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedEntityCore : MonoBehaviour
{
    #region Variables

    /* === Advanced Entity Core Settings === */

    #region Core Settings

    [Header("Core Settings")]
    public CharacterController controller;
    public Transform self;
    public Transform head;
    private Transform localSpace => self.transform;

    #endregion

    #region Input Settings

    [Header("Input Settings")]
    public float xSensitivity = 1f;
    public float ySensitivity = 1f;
    private float _cameraPitch = 0f;
    private Vector3 _moveInput;
    public bool hasMoveInput;

    #endregion

    #region Cround Check

    [Header("Ground Check")]
    public bool doGroundCheck = true;
    public Transform gCTransform;
    public LayerMask gCCollisionMask;
    public float gCRadius = 0.5f;

    #endregion

    #region Movement Settings

    [Header("Movement Settings")]
    public GMoveStruct jogging;
    private GMoveStruct _currentGMove;
    public GMoveStruct CurrentGMove
    {
        get => _currentGMove;
        set => _currentGMove = value;
    }

    #endregion

    #region Properties

    // Grounded
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;

    // Gait
    private EGait _gait;
    public EGait Gait
    {
        get => _gait;
        set => _gait = value;
    }
    
    // Velocities
    private Vector2 xzVelocity => new Vector2(_velocity.x, _velocity.z);
    private float yVelocity => controller.velocity.y;
    
    private Vector3 _velocity = Vector3.zero;
    public Vector3 inputVelocity = Vector3.zero;

    #endregion
    
    /* === Advanced Entity Core Settings === */
    
    #endregion
    
    public AdvancedEntityCore()
    {
        jogging = new GMoveStruct(2f, 5f, 15f, 2f, 5f);
        _currentGMove = jogging;
    }
    
    #region Input Messages

    // Look input here
    public void OnLook(InputValue value)
    {
        Vector2 inputVec = value.Get<Vector2>();
        float xInput = inputVec.x * 0.075f * xSensitivity;
        float yInput = inputVec.y * 0.075f * ySensitivity;

        _cameraPitch -= yInput;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        
        head.localEulerAngles = Vector3.right * _cameraPitch;
        self.Rotate(Vector3.up * xInput);
    }

    // Movement input here
    public void OnMove(InputValue value)
    {
        Vector2 inputVec = value.Get<Vector2>();
        hasMoveInput = Mathf.Abs(inputVec.x) > 0.01f || Mathf.Abs(inputVec.y) > 0.01f;
        _moveInput = new Vector3(inputVec.x, 0f, inputVec.y).normalized;
    }
    
    #endregion

    #region Messages

    /* === Sprint Component messages === */
    public void OnEndSprint()
    {
        _currentGMove = jogging;
        _gait = EGait.Jogging;
    }
    /* === Sprint Component messages === */

    #endregion

    #region Functions
    

    // Check if the ground status changes.
    private void CheckGround()
    {
        // Quick escape.
        if (!doGroundCheck) return;
        
        bool newIsGrounded = Physics.CheckSphere(gCTransform.position, gCRadius, gCCollisionMask);
        if (newIsGrounded != _isGrounded)
        {
            _isGrounded = newIsGrounded;
            if (newIsGrounded)
            {
                BroadcastMessage("OnLanded");
            }
            else
            {
                // BroadcastMessage("OnInAir");
                Debug.Log("I was supposed to broadcast OnInAir");
            }
        }
    }

    #region Movement

    #region Grounded Movement

    private void CalcGBrakingForces(bool isExceedingMaxSpeed)
    {
        // Reset inputVelocity to prevent old input from adding to new input.
        inputVelocity = Vector3.zero;

        // Quick exit.
        if (_velocity == Vector3.zero) return;
        
        // Calculate braking forces.
        Vector2 brakingDirection = (-xzVelocity).normalized;
        float frictionComponent = _currentGMove.brakingFriction * Time.deltaTime;
        Vector2 deltaXZVelocity = (_currentGMove.brakingDeceleration * frictionComponent) * brakingDirection;
        Vector2 newXZVelocity = xzVelocity + deltaXZVelocity;
        
        // If was exceeding max speed but no longer, adjust.
        if (isExceedingMaxSpeed && (newXZVelocity.sqrMagnitude < Mathf.Pow(_currentGMove.maxSpeed, 2)))
        {
            newXZVelocity = newXZVelocity.normalized * _currentGMove.maxSpeed;
        }
        
        // If braking forced a reverse change in direction, set to zero.
        if (Vector2.Dot(newXZVelocity, xzVelocity) < 0)
        {
            _velocity = Vector3.zero;
        }
        else
        {
            // TODO: Make compatible with custom gravity direction.
            // Will need to map world space velocity to local space, then grab only XZ component.
            _velocity = new Vector3(newXZVelocity.x, 0f, newXZVelocity.y);
        }
    }

    private void CalcGInputForces()
    {
        // Lateral movement calc
        // x then z due to Unity's different coord layout
        float rotTargetAngle = Mathf.Atan2(_moveInput.x, _moveInput.z) * Mathf.Rad2Deg + self.eulerAngles.y;
        Vector3 moveDirection = Quaternion.Euler(0f, rotTargetAngle, 0f) * Vector3.forward;

        float frictionComponent = _currentGMove.friction * Time.deltaTime;
        Vector3 moveComponent = moveDirection * (_currentGMove.acceleration * frictionComponent);
        Vector3 velocityComponent = (1f - frictionComponent) * _velocity;

        float ratio = (Vector3.Dot(moveDirection.normalized, _velocity.normalized) - 3f) / -2f;
        
        inputVelocity = velocityComponent + (moveComponent * ratio);

        // If exceeding max speed, adjust.
        Vector3 newVelocity = inputVelocity;
        if (newVelocity.sqrMagnitude > Mathf.Pow(_currentGMove.maxSpeed, 2))
        {
            newVelocity = newVelocity.normalized * _currentGMove.maxSpeed;
        }

        _velocity = newVelocity;
    }
    
    private void CalcGMovement()
    {
        // Square magnitude avoids costly square root process of magnitude.
        bool isExceedingMaxSpeed = xzVelocity.sqrMagnitude > Mathf.Pow(_currentGMove.maxSpeed, 2);

        if (isExceedingMaxSpeed || !hasMoveInput)
        {
            CalcGBrakingForces(isExceedingMaxSpeed);
        }

        if (hasMoveInput) { CalcGInputForces(); }
    }

    #endregion

    #region Falling Movement

    private void CalcFMovement()
    {
        
    }

    #endregion

    // Master Calculate Movement function
    private void CalcMovement()
    {
        if (IsGrounded) { CalcGMovement(); }
        else { CalcFMovement(); }
        
        controller.Move(_velocity * Time.deltaTime);
    }

    #endregion

    #endregion

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        CalcMovement();
    }
}

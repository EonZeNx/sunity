using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : NetworkedBehaviour
{
    // TODO: Figure out Unity and/or C# comment system and describe wtf is going on here
    // TODO: Implement update/fix for Cinemachine for good 3PC
    // TODO: More advanced movement system

    #region Variables
    [Header("External References")]
    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    
    // Variables
    [Header("Input Calculation")]
    [ReadOnly]
    protected Vector3 LastMoveInput;
    
    [Header("Movement")]
    public Vector3 velocity;
    public float gravity;
    public bool isGrounded;
    protected float MoveSpeed;
    
    [Header("Ground Collision Checks")]
    public float groundCheckRadius;
    public LayerMask groundCollisionMask;

    [Header("Jumping")]
    protected Vector3 JumpVec;
    public float jumpForce;
    public bool tryingToJump;

    [Header("Rotation")]
    public float rotSmoothTime;
    private float _rotSmoothVel;
    #endregion

    // Constructor
    public ThirdPersonMovement()
    {
        gravity = -9.81f;
        MoveSpeed = 6f;
        LastMoveInput = new Vector3();

        groundCheckRadius = 0.4f;
        
        JumpVec = Vector3.up;
        jumpForce = 20f;
        tryingToJump = false;

        rotSmoothTime = 0.03f;
    }
    
    // Functions
    #region Input Events
    public void OnMove(InputValue value)
    {
        // Basic movement logic
        var moveVec = value.Get<Vector2>();
        LastMoveInput = new Vector3(moveVec.x, 0f, moveVec.y).normalized;
    }
    
    public void OnJump()
    {
        tryingToJump = true;
    }
    #endregion
    
    #region Movement Functions
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundCollisionMask);
    }
    
    private void Gravity()
    {
        if (tryingToJump)
        {
            if (isGrounded)
            {
                // Jump-height based formula
                // Sqrt is surprisingly performance draining, thus traditional jump force
                // velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                
                velocity.y = jumpForce;
            }
            tryingToJump = false;
        }
        else if (isGrounded && velocity.y < 0f) { velocity.y = -2f; }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Movement()
    {
        if (LastMoveInput.magnitude < 0.1f) { return; }
        
        // // x then z due to Unity's different coord layout
        float rotTargetAngle = Mathf.Atan2(LastMoveInput.x, LastMoveInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDirection = Quaternion.Euler(0f, rotTargetAngle, 0f) * Vector3.forward;
        controller.Move(MoveSpeed * Time.deltaTime * moveDirection.normalized);
    }
    #endregion
    
    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Gravity();
        Movement();
    }
}

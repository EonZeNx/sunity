using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AdvancedMovement;

public class GravityComponent : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public AdvancedEntityCore advEntityCore;
    private CharacterController controller
    {
        get { return advEntityCore.controller; }
    }
    private bool isGrounded
    {
        get { return advEntityCore.IsGrounded; }
    }
    
    [Header("Gravity Settings")]
    public GravityStruct gStruct = new GravityStruct(Vector3.up);
    public bool gravityEnabled = true;
    
    [Header("Jump Settings")]
    public JumpStruct jStruct = new JumpStruct(2f, 2);
    public bool jumpEnabled = true;
    
    private Vector3 _verticalForces = Vector3.zero;
    private bool _hasPendingJump = false;
    private int _currentJumps = 0;

    #endregion
    
    #region Utils
    private void Awake()
    {
        // Auto-setter just in case.
        if (advEntityCore == null)
        {
            advEntityCore = GetComponent<AdvancedEntityCore>();
        }
    }

    public Vector3 GetWorldRight()
    {
        return Vector3.Cross(controller.transform.forward, gStruct.worldUp);
    }
    
    public Vector3 GetWorldForward()
    {
        return controller.transform.forward;
    }

    public Vector3 GetWorldUp()
    {
        return gStruct.worldUp;
    }
    
    #endregion

    #region Messages

    // Jump the player next update.
    public void OnJump()
    {
        if (!jumpEnabled || _hasPendingJump) return;
        if (_currentJumps > 0 || jStruct.maxJumps == -1)
        {
            _hasPendingJump = true;
        }
    }

    // Receive message broadcast for OnLanded. 
    public void OnLanded()
    {
        _currentJumps = jStruct.maxJumps;
    }
    
    #endregion
    
    // Update is called once per frame
    void Update()
    {
        // Quick escape.
        if (!gravityEnabled) return;
        
        // Calculate gravity.
        float deltaGravityForce = gStruct.gravity * Time.deltaTime;
        Vector3 deltaGravity = gStruct.worldUp * deltaGravityForce;

        // Jump the character.
        if (jumpEnabled && _hasPendingJump)
        {
            _verticalForces = gStruct.worldUp * jStruct.force;
            _hasPendingJump = false;
            _currentJumps -= 1;
        }
        else
        {
            _verticalForces = isGrounded ? deltaGravity : _verticalForces + deltaGravity;
        }
        
        advEntityCore.controller.Move(_verticalForces * Time.deltaTime);
    }
}

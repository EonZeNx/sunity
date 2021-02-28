using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprintComponent : MonoBehaviour
{
    #region Variables
    
    [Header("References")]
    public AdvancedEntityCore advEntityCore;

    [Header("Sprint settings")]
    public bool sprintEnabled = true;
    public GMoveStruct sprinting;

    #region Properties

    public bool IsSprinting
    {
        get { return advEntityCore.Gait == EGait.Sprinting; }
    }

    #endregion

    #endregion

    #region Utils
    
    public SprintComponent()
    {
        sprinting = new GMoveStruct(2f, 15f, 30f, 2f, 5f);
    }
    
    private void Awake()
    {
        // Auto-setter just in case.
        if (advEntityCore == null)
        {
            advEntityCore = GetComponent<AdvancedEntityCore>();
        }
    }
    
    #endregion

    #region Input Messages

    public void OnSprint(InputValue value)
    {
        float sprintValue = value.Get<float>();
        bool newIsSprinting = sprintValue > 0.5f;
        
        // Quick exit.
        if (newIsSprinting == IsSprinting) return;

        if (newIsSprinting)
        {
            BroadcastMessage("OnBeginSprint");
        }
        else
        {
            BroadcastMessage("OnEndSprint");
        }
    }

    #endregion

    #region Messages

    private void OnBeginSprint()
    {
        // Quick exit.
        if (!sprintEnabled) return;

        advEntityCore.CurrentGMove = sprinting;
        advEntityCore.Gait = EGait.Sprinting;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityCore : MonoBehaviour
{
    #region Variables
    [Header("External References")]

    [Header("Input Variables")]
    public Vector3 lastInputVec;
    #endregion

    public EntityCore()
    {
    }
    
    // Functions
    #region Input Events
    public void OnMove(InputValue value)
    {
        Vector2 inputVec = value.Get<Vector2>();
        lastInputVec = new Vector3(inputVec.x, 0f, inputVec.y).normalized;
        
        BroadcastMessage("OnMoveInput", lastInputVec);
    }
    
    public void OnJump()
    {
        BroadcastMessage("OnJumpInput");
    }
    
    public void OnSecondaryAction(InputValue value)
    {
        BroadcastMessage("OnAimInput", value);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}

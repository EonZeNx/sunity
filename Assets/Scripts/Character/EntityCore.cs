using System.Collections;
using System.Collections.Generic;
using Character;
using MLAPI;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityCore : NetworkedBehaviour
{
    #region Variables
    [Header("External References")]

    [Header("Input Variables")]
    public Vector3 lastInputVec;
    #endregion

    public GameObject EntityCamera;

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

    public override void NetworkStart()
    {
        if (!IsLocalPlayer)
        {
            // Disable camera
            EntityCamera.SetActive(false);
            Debug.Log($"Camera for this player {this.NetworkId} has been disabled.");

            // Disable player input
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

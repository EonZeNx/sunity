using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IndependentBodyRotation : NetworkBehaviour
{
    /// <summary>
    /// References to the independently rotating parts
    /// </summary>
    [Header("Independent bodies")]
    public Transform upper;
    public Transform lower;

    [Header("Follow objects")]
    public CharacterController controller;
    public Transform cam;

    private Vector3 _lastVel;

    /// <summary>
    /// Aiming variables for upper body
    /// </summary>
    [Header("Aiming variables")]
    public bool isAiming = true;
    public float timeSinceLastAim = 0f;
    public float maxTimeSinceLastAim = 5f;
    public bool wasAiming = false;
    
    /// <summary>
    /// Smoothness for both velocity and time
    /// </summary>
    [Header("Smoothing variables")]
    public float smoothTimeUpperBody = 0.02f;
    public float smoothTimeLowerBody = 0.07f;
    private float _smoothVelUpperBody;
    private float _smoothVelLowerBody;
    
    public void OnAimInput(InputValue value)
    {
        // Follow camera
        isAiming = value.Get<float>() > 0.5;
        if (isAiming)
        {
            wasAiming = true;
            timeSinceLastAim = 0f;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (!isAiming && wasAiming)
        {
            timeSinceLastAim += Time.deltaTime;
            if (timeSinceLastAim > maxTimeSinceLastAim)
            {
                timeSinceLastAim = 0f;
                wasAiming = false;
            }
        }
        
        // Get current angles
        float currentUpperAngle = upper.transform.eulerAngles.y;
        float currentLowerAngle = lower.transform.eulerAngles.y;
        
        // Calculate target angles
        float targetUpperAngle = cam.transform.eulerAngles.y;
        Vector3 playerVelocity = controller.velocity;
        playerVelocity.y = 0;
        playerVelocity = playerVelocity.normalized;
        
        if (playerVelocity.magnitude > 0.05f) { _lastVel = playerVelocity; }
        float targetLowerAngle = Mathf.Atan2(_lastVel.x, _lastVel.z) * Mathf.Rad2Deg;

        // Lower body smooth and apply
        float transitionLowerBodyAngle = Mathf.SmoothDampAngle(currentLowerAngle, targetLowerAngle, ref _smoothVelLowerBody, smoothTimeLowerBody);
        lower.transform.Rotate(Vector3.up, transitionLowerBodyAngle - currentLowerAngle);

        // Upper body smooth and apply
        float transitionUpperBodyAngle = wasAiming ? 
            Mathf.SmoothDampAngle(currentUpperAngle, targetUpperAngle, ref _smoothVelUpperBody, smoothTimeUpperBody) : 
            Mathf.SmoothDampAngle(currentUpperAngle, currentLowerAngle, ref _smoothVelUpperBody, smoothTimeUpperBody);
        upper.transform.Rotate(Vector3.up, transitionUpperBodyAngle - currentUpperAngle);
    }
}

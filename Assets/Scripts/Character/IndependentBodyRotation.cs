using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependentBodyRotation : MonoBehaviour
{
    /// <summary>
    /// References to the independently rotating parts
    /// </summary>
    public Transform upper;
    public Transform lower;

    public CharacterController controller;
    public Transform cam;

    /// <summary>
    /// Smoothness for both velocity and time
    /// </summary>
    public float smoothVelUpperBody;
    public float smoothVelLowerBody;
    public float smoothTimeUpperBody = 0.02f;
    public float smoothTimeLowerBody = 0.07f;

    public void Update()
    {
        // Get current angles
        float currentUpperAngle = upper.transform.eulerAngles.y;
        float currentLowerAngle = lower.transform.eulerAngles.y;
        
        // Calculate target angles
        float targetUpperAngle = cam.transform.eulerAngles.y;
        Vector3 playerVelocity = controller.velocity;
        playerVelocity.y = 0;
        float targetLowerAngle = Mathf.Atan2(playerVelocity.x, playerVelocity.z) * Mathf.Rad2Deg;

        // Smooth transitions
        float targetUpperBodyAngle = Mathf.SmoothDampAngle(currentUpperAngle, targetUpperAngle, ref smoothVelUpperBody, smoothTimeUpperBody);
        float targetLowerBodyAngle = Mathf.SmoothDampAngle(currentLowerAngle, targetLowerAngle, ref smoothVelLowerBody, smoothTimeLowerBody);

        // Apply angles
        upper.transform.Rotate(Vector3.up, targetUpperBodyAngle - currentUpperAngle);
        lower.transform.Rotate(Vector3.up, targetLowerBodyAngle - currentLowerAngle);
    }
}

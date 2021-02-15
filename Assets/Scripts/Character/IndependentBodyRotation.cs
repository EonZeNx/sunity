using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependentBodyRotation : MonoBehaviour
{
    /// <summary>
    /// References to the independently rotating parts
    /// </summary>
    public Transform upperBody;
    public Transform lowerBody;

    public CharacterController controller;
    public Transform cam;

    /// <summary>
    /// Target vectors for both parts of the body
    /// </summary>
    public Vector3 targetUpperBodyVec;
    public Vector3 targetLowerBodyVec;

    /// <summary>
    /// Smoothness for both velocity and time
    /// </summary>
    public float smoothVelUpperBody;
    public float smoothVelLowerBody;
    public float smoothTimeUpperBody = 0.05f;
    public float smoothTimeLowerBody = 0.03f;

    public void Update()
    {
        float currentUpperBodyAngle = upperBody.transform.eulerAngles.y;
        float currentLowerBodyAngle = lowerBody.transform.eulerAngles.y;
        float lookAngle = cam.transform.eulerAngles.y;
        
        Vector3 playerVelocity = controller.velocity;
        playerVelocity.y = 0;
        float playerVelocityAngle = Mathf.Atan2(playerVelocity.x, playerVelocity.z) * Mathf.Rad2Deg;

        float targetUpperBodyAngle = Mathf.SmoothDampAngle(currentUpperBodyAngle, lookAngle, ref smoothVelUpperBody, smoothTimeUpperBody);
        float targetLowerBodyAngle = Mathf.SmoothDampAngle(currentLowerBodyAngle, playerVelocityAngle, ref smoothVelLowerBody, smoothTimeLowerBody);

        upperBody.transform.Rotate(Vector3.up, targetUpperBodyAngle - currentUpperBodyAngle);
        lowerBody.transform.Rotate(Vector3.up, targetLowerBodyAngle - currentLowerBodyAngle);
    }
}

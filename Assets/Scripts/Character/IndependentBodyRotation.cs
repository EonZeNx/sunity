using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependentBodyRotation : MonoBehaviour
{
    /// <summary>
    /// References to the independently rotating parts
    /// </summary>
    public GameObject upperBody;
    public GameObject lowerBody;

    public CharacterController controller;
    public Camera cam;

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
        float targetAngle = cam.transform.eulerAngles.y;
        float actualAngle = upperBody.transform.eulerAngles.y;
        
        float rotUpperBodyAngle = Mathf.SmoothDampAngle(gameObject.transform.eulerAngles.y, targetAngle - actualAngle,
            ref smoothVelUpperBody, smoothTimeUpperBody);
        
        upperBody.transform.Rotate(Vector3.up, rotUpperBodyAngle * Time.deltaTime);
    }
}

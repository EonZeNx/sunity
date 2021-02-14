using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    // External references
    public CharacterController controller;
    public Transform cam;
    
    // Variables
    public float walkSpeed = 8f;
    public float rotSmoothTime = 0.04f;
    private float _rotSmoothVel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xVec = Input.GetAxisRaw("Horizontal");
        float zVec = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(xVec, 0f, zVec).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // x then z due to Unity's different coord layout
            float rotTargetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float rotActualAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotTargetAngle,
                ref _rotSmoothVel, rotSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotActualAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, rotTargetAngle, 0f) * Vector3.forward;
            controller.Move(walkSpeed * Time.deltaTime * moveDirection.normalized);
        }
    }
}

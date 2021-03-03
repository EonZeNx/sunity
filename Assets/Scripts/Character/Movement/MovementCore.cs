using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedMovement;
using Character;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementCore : MonoBehaviour
{
	#region Variables

	#region References

	public Transform aimTransform;
	public Vector3 AimDirection => aimTransform.forward;
	public Rigidbody physics;
	private float _cameraPitch = 0f;

	#endregion

	#region Config

	public float xSensitivity = 1f;
	public float ySensitivity = 1f;
	
	#region Movement Config

	public JumpStruct JStruct;
	public GMoveStruct GStruct;

	#endregion

	#endregion

	#region Hits

	public LayerMask GroundLayers;
	private RaycastHit HitDown;

	#endregion
	
	#region Status

	// Grounded
	private bool _isGrounded = false;
	public bool IsGrounded => _isGrounded;
	public bool IsInAir => !_isGrounded;
	
	// Movement input
	private Vector3 _moveInput;
	public bool hasMoveInput => Mathf.Abs(_moveInput.x) > 0.05f || Mathf.Abs(_moveInput.z) > 0.05f;
	
	// Movement
	// // Jumping
	private int _currentJumps;
	private JumpStruct _currentJStruct;
	public JumpStruct currentJStruct => _currentJStruct;
	
	// // Grounded movement
	private GMoveStruct _currentGStruct;
	public GMoveStruct currentGStruct => _currentGStruct;

	#endregion
	
	#region Velocity

	public Vector3 Velocity => physics.velocity;
	public Vector3 XZVelocity => new Vector3(Velocity.x, 0f, Velocity.z);
	public float YVelocity => Velocity.y;

	public float Speed => Velocity.magnitude;
	public float XZSpeed => XZVelocity.magnitude;
	public float YSpeed => YVelocity;

	#endregion

	#endregion

	
	/// <summary>
	/// Constructor.
	/// </summary>
	public void MovementComponent()
	{
		// References
		physics = GetComponent<Rigidbody>();
		
		// Movement
		// // Jumping
		JStruct = new JumpStruct(5f, 2);
		_currentJStruct = JStruct;
		_currentJumps = JStruct.maxJumps;

		// // Grounded Movement
		GStruct = new GMoveStruct(1f, 8f, 16f, 1f, 8f);
		_currentGStruct = GStruct;
	}
	
	
	#region Functions
	
	#region Messages

	#region Input Messages

	/// <summary>
	/// Basic movement message.
	/// </summary>
	/// <param name="value"></param>
	public void OnMove(InputValue value)
	{
		Vector2 inputVec = value.Get<Vector2>();
		_moveInput = new Vector3(inputVec.x, 0f, inputVec.y).normalized;
	}
	
	/// <summary>
	/// Basic camera message.
	/// </summary>
	/// <param name="value"></param>
	public void OnLook(InputValue value)
	{
		Vector2 inputVec = value.Get<Vector2>();
		float xInput = inputVec.x * 0.075f * xSensitivity;
		float yInput = inputVec.y * 0.075f * ySensitivity;

		// Use absolute values to clamp pitch to +-90 degrees.
		_cameraPitch -= yInput;
		_cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        
		aimTransform.localEulerAngles = Vector3.right * _cameraPitch;
		transform.Rotate(Vector3.up * xInput);
	}

	/// <summary>
	/// Jump message.
	/// </summary>
	/// <param name="value">Check if the entity is pressing or releasing the input.</param>
	public void OnJump(InputValue value)
	{
		float jumpValue = value.Get<float>();
		bool pressedJump = jumpValue > 0.5f;

		if (!pressedJump) return;

		if (_currentJumps > 0 || _currentJStruct.maxJumps == -1)
		{
			if (_currentJStruct.maxJumps != -1) _currentJumps -= 1;
			physics.velocity = new Vector3(Velocity.x, _currentJStruct.force, Velocity.z);
		}
	}

	#endregion

	/// <summary>
	/// Receive message for landing on the ground.
	/// </summary>
	private void OnLanded()
	{
		Debug.Log("Called OnLanded");

		_currentJumps = _currentJStruct.maxJumps;
	}

	/// <summary>
	/// Receive message for leaving the ground.
	/// </summary>
	private void OnLeaveGround()
	{
		Debug.Log("Called OnLeaveGround");
	}

	#endregion

	
	#region Utils

	

	#endregion

	
	#region Updates

	/// <summary>
	/// Check if we're still on the ground.
	/// </summary>
	private void GroundCheck()
	{
		bool newIsGrounded = Physics.Raycast(transform.position, transform.up * -1, out HitDown, 1.15f, GroundLayers);
		if (newIsGrounded != _isGrounded)
		{
			_isGrounded = newIsGrounded;
			if (newIsGrounded)
			{
				BroadcastMessage("OnLanded");
			}
			else
			{
				BroadcastMessage("OnLeaveGround");
			}
		}
	}

	#endregion


	#region Movement Functions

	#region Grounded Movement

	/// <summary>
	/// Grounded input movement function.
	/// </summary>
	private void GInputMovement()
	{
		// Lateral movement calc
		// x then z due to Unity's different coord layout
		float rotTargetAngle = Mathf.Atan2(_moveInput.x, _moveInput.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
		Vector3 moveDirection = Quaternion.Euler(0f, rotTargetAngle, 0f) * Vector3.forward;
		
		// Friction lerp for direction
		Vector3 newDirection = Vector3.Lerp(
			XZVelocity.normalized, 
			moveDirection, 
			_currentGStruct.friction);

		float newXZSpeed = XZSpeed + _currentGStruct.acceleration;
		newXZSpeed = Mathf.Clamp(newXZSpeed, 0f, _currentGStruct.maxSpeed);
		
		// Friction lerp for XZSpeed
		newXZSpeed = Mathf.Lerp(
			XZSpeed, 
			newXZSpeed, 
			_currentGStruct.friction);
		
		physics.velocity += newDirection * (newXZSpeed * Time.deltaTime);
	}
	
	/// <summary>
	/// Grounded braking function.
	/// </summary>
	private void GBrakingMovement()
	{
		Vector3 brakingDirection = (-XZVelocity).normalized;

		float brakingMagnitude = Mathf.Lerp(
			0f, 
			_currentGStruct.brakingDeceleration * Time.deltaTime, 
			_currentGStruct.brakingFriction);

		Vector3 brakingForce = brakingDirection * brakingMagnitude;

		// Check if the entity has changed direction.
		if (Vector3.Dot(XZVelocity, (XZVelocity + brakingForce).normalized) < 0)
		{
			physics.velocity = Vector3.zero;
		}
		else
		{
			physics.velocity += brakingForce;
		}
	}

	/// <summary>
	/// Main grounded movement input.
	/// </summary>
	private void GMovement()
	{
		if (hasMoveInput)
		{
			GInputMovement();
		}
		else
		{
			GBrakingMovement();
		}
	}

	#endregion

	#region Falling Movement

	private void FMovement()
	{
		
	}

	#endregion

	/// <summary>
	/// Main movement function.
	/// </summary>
	private void Movement()
	{
		if (_isGrounded)
		{
			GMovement();
		}
		else
		{
			FMovement();
		}
		
	}

	#endregion
	
	
	#region Overrides

	/// <summary>
	/// Start is called before the first frame update
	/// </summary>
	void Start()
	{
		// Movement
		// // Jumping
		JStruct = new JumpStruct(5f, 2);
		_currentJStruct = JStruct;
		_currentJumps = JStruct.maxJumps;

		// // Grounded Movement
		GStruct = new GMoveStruct(1f, 32f, 16f, 0f, 32f);
		_currentGStruct = GStruct;
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update()
	{
		GroundCheck();
		Movement();
	}

	#endregion
	
	#endregion
}

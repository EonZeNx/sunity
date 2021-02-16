using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Character
{
	#region Structs
	public struct GlobalMovementStruct
	{
		// Gravity force per delta time
		public float Gravity { get; set; }

		// Reverse of gravity direction. This ensures consistence when dealing with other aspects of Unity
		public Vector3 ReverseGravityDirection { get; set; }

		// Whether or not to rotate the character to the current gravity
		public bool RotateCharacterToGravity { get; set; }

		// Maximum speed when launching or falling
		public float TerminalVelocity { get; set; }

		public GlobalMovementStruct(Vector3 reverseGravityDirection, float terminalVelocity, float gravity = -9.81f, 
			bool rotateCharacterToGravity = false)
		{
			Gravity = gravity;
			ReverseGravityDirection = reverseGravityDirection;
			RotateCharacterToGravity = rotateCharacterToGravity;
			TerminalVelocity = terminalVelocity;
		}
	}

	public struct BasicMovementStruct
	{
		// How much control when changing directions
		public float GroundFriction { get; set; }

		// Maximum speed while moving
		public float MaxSpeed { get; set; }

		// Maximum acceleration while moving
		public float MaxAcceleration { get; set; }

		// Maximum deceleration while moving
		public float MaxBrakingDeceleration { get; set; }

		// Friction applied when acceleration == 0 or exceeding max speed
		public float BrakingFriction { get; set; }

		// Optional friction factor when calculating actual friction
		public float BrakingFrictionFactor { get; set; }

		public BasicMovementStruct(float groundFriction, float maxSpeed, float maxAcceleration, 
			float maxBrakingDeceleration, float brakingFriction, float brakingFrictionFactor)
		{
			GroundFriction = groundFriction;
			MaxSpeed = maxSpeed;
			MaxAcceleration = maxAcceleration;
			MaxBrakingDeceleration = maxBrakingDeceleration;
			BrakingFriction = brakingFriction;
			BrakingFrictionFactor = brakingFrictionFactor;
		}
	}

	public struct AirMovementStruct
	{
		public float Control { get; set; }
		public float ControlBoostMultiplier { get; set; }
		public float ControlBoostVelocityThreshold { get; set; }

		public AirMovementStruct(float control, float controlBoostMultiplier, float controlBoostVelocityThreshold)
		{
			Control = control;
			ControlBoostMultiplier = controlBoostMultiplier;
			ControlBoostVelocityThreshold = controlBoostVelocityThreshold;
		}
	}
	
	public struct JumpMovementStruct
	{
		public float Force { get; set; }
		public int MaxNumberOfJumps { get; set; }

		public JumpMovementStruct(float force, int maxNumberOfJumps)
		{
			Force = force;
			MaxNumberOfJumps = maxNumberOfJumps;
		}
	}
	#endregion

	#region Enums
	public enum MovementModeEnum
	{
		Grounded,
		Falling
	}

	public enum GaitEnum
	{
		Walk,
		Run
	}

	public enum StanceEnum
	{
		Crouched,
		Standing
	}
	#endregion

	public class MovementComponent : MonoBehaviour
	{
		#region Variables
		[Header("External References")]
		public CharacterController controller;
		public Transform cam;
		public Transform groundCheckPosition;
		
		[Header("Ground Check Variables")]
		public float groundCheckRadius;
		public LayerMask groundCollisionMask;
		
		protected GlobalMovementStruct GlobalMovement { get; }
		protected BasicMovementStruct BasicMovement { get; }
		protected AirMovementStruct AirMovement { get; }
		protected JumpMovementStruct JumpMovement { get; set; }

		protected MovementModeEnum MovementMode { get; set; }
		protected GaitEnum Gait { get; }
		protected StanceEnum Stance { get; }

		protected int CurrentNumberOfJumps { get; set; }
		protected Vector3 Acceleration { get; set; }
		protected Vector3 OldVelocity { get; set; }
		protected Vector3 Velocity { get; set; }
		protected Vector3 NewVelocity { get; set; }
		protected Vector3 GravityVec { get; set; }
		protected List<Vector3> PendingLaunches { get; set; }
		protected Vector3 LastMoveInput;
		#endregion

		// Constructor
		public MovementComponent()
		{
			GlobalMovement = new GlobalMovementStruct(Vector3.up, 20f, -9.81f, false);
			BasicMovement = new BasicMovementStruct(8f, 600f, 2048f, 2048f, 2048f, 1.5f);
			AirMovement = new AirMovementStruct(0.05f, 2f, 25f);
			JumpMovement = new JumpMovementStruct(20f, 2);

			MovementMode = MovementModeEnum.Falling;
			Gait = GaitEnum.Walk;
			Stance = StanceEnum.Standing;

			PendingLaunches = new List<Vector3>();
			CurrentNumberOfJumps = JumpMovement.MaxNumberOfJumps;
			groundCheckRadius = 0.4f;

			LastMoveInput = Vector3.zero;
		}

        #region Utils
        public void AddLaunch(Vector3 launch, bool isAcceleration = false)
        {
	        if (isAcceleration)
	        {
		        PendingLaunches.Add(launch * (Time.deltaTime * Time.deltaTime));
		        return;
	        }
	        PendingLaunches.Add(launch);
        }
        
        public bool IsExceedingMaxSpeed(float maxSpeed)
        {
	        maxSpeed = Mathf.Max(0f, maxSpeed);

			// Error tolerance
			float ErrorTolerancePercent = 1.01f;
			return Velocity.sqrMagnitude > maxSpeed * maxSpeed * ErrorTolerancePercent;
		}
        public bool IsGrounded()
        {
	        return MovementMode == MovementModeEnum.Grounded;
        }
        public bool IsFalling()
        {
	        return MovementMode == MovementModeEnum.Falling;
        }
		#endregion
		
		#region Events/Messages
		public void OnReceiveMoveInput(Vector3 lastMoveInput)
		{
			LastMoveInput = lastMoveInput;
		}
		
		public void OnReceiveJumpInput()
		{
			Debug.Log("OnJump");
			NewVelocity += JumpMovement.Force * GlobalMovement.ReverseGravityDirection;
			// if (CurrentNumberOfJumps > 0)
			// {
			// 	Velocity += JumpMovement.Force * GlobalMovement.ReverseGravityDirection;
			// 	CurrentNumberOfJumps -= 1;
			// }
		}

		public void OnLanded()
		{
			Debug.Log("OnLanded");
			CurrentNumberOfJumps = JumpMovement.MaxNumberOfJumps;
		}

		public void OnMovementModeUpdate(MovementModeEnum oldMode)
		{
			Debug.Log($"Updated MovementMode from {oldMode} to {MovementMode}");
		}
		#endregion
		
		private bool GroundCheck()
		{
			return Physics.CheckSphere(groundCheckPosition.position, groundCheckRadius, groundCollisionMask);
		}
		public void CalcMovementMode()
		{
			MovementModeEnum newMovementMode = GroundCheck() ? MovementModeEnum.Grounded : MovementModeEnum.Falling;
			if (newMovementMode == MovementMode)
			{
				return;
			}
			if (MovementMode != MovementModeEnum.Grounded && newMovementMode == MovementModeEnum.Grounded)
			{
				BroadcastMessage("OnLanded");
			}
			MovementMode = newMovementMode;
			
			// if (newMovementMode != MovementMode) { BroadcastMessage("OnMovementModeUpdate", oldMovementMode); }
		}
		public void ApplyPendingLaunches()
        {
			OldVelocity = Velocity;
			Vector3 deltaVelocity = Vector3.zero;
			foreach (var pendingLaunch in PendingLaunches)
			{
				deltaVelocity += pendingLaunch;
			}
			PendingLaunches.Clear();

			Velocity = deltaVelocity;
			Velocity = Velocity.magnitude > GlobalMovement.TerminalVelocity 
				? Velocity.normalized * GlobalMovement.TerminalVelocity
				: Velocity;
        }

		#region Velocity
		public void ApplyBrakingVelocity(float dt, float friction, float brakingDeceleration)
        {
			float frictionFactor = Mathf.Max(0f, BasicMovement.BrakingFrictionFactor);
			friction = Mathf.Max(0f, friction * frictionFactor);

			bool hasZeroFriction = friction == 0f;
			bool hasZeroBraking = brakingDeceleration == 0f;
			if (hasZeroFriction && hasZeroBraking) { return; }

			Vector3 oldVelocity = Velocity;
			Vector3 revAccel = hasZeroBraking ? Vector3.zero : (-brakingDeceleration * Velocity.normalized);
			Velocity += (-friction * Velocity + revAccel) * dt;

			if (Vector3.Dot(Velocity, oldVelocity) <= 0f) { Velocity = Vector3.zero; }
		}

		public void CalcVelocityGrounded(float dt)
		{
			Vector3 oldVelocity = Velocity;

			float friction = Mathf.Max(0f, BasicMovement.GroundFriction);
			float maxSpeed = BasicMovement.MaxSpeed;

			bool hasZeroAcceleration = Acceleration.magnitude <= 0.01f;
			bool velocityOverMax = IsExceedingMaxSpeed(maxSpeed);

			if (hasZeroAcceleration || velocityOverMax)
			{
				ApplyBrakingVelocity(dt, friction, BasicMovement.MaxBrakingDeceleration);

				if (velocityOverMax && Velocity.sqrMagnitude < (maxSpeed * maxSpeed) && Vector3.Dot(Acceleration, oldVelocity) > 0.0f)
				{
					Velocity = oldVelocity.normalized * maxSpeed;
				}
			}
		}

		public void CalcVelocity(float dt)
        {
			switch (MovementMode)
			{
				case MovementModeEnum.Grounded:
					CalcVelocityGrounded(dt);
					break;
				/*case MovementModeEnum.Falling:
					CalcVelocityFalling(dt);
					break;*/
				default:
					CalcVelocityGrounded(dt);
					break;
			}
		}

		public void CalcAcceleration()
        {
			Acceleration = Velocity - OldVelocity;
        }
		#endregion
		
		private void CalcGravity()
		{
			if (IsGrounded())
			{
				GravityVec = -2f * GlobalMovement.ReverseGravityDirection;
			}
			else
			{
				GravityVec += GlobalMovement.ReverseGravityDirection * (GlobalMovement.Gravity);
			}
			controller.Move(GravityVec * (Time.deltaTime * Time.deltaTime));
		}

		public void Update()
		{
	        CalcGravity();
	        if (GroundCheck())
	        {
		        MovementMode = MovementModeEnum.Grounded;
	        }
	        
	        controller.Move(NewVelocity * Time.deltaTime);
	        OldVelocity = Velocity;
	        Velocity = NewVelocity;
	        NewVelocity = Vector3.zero;
		}
	}
}

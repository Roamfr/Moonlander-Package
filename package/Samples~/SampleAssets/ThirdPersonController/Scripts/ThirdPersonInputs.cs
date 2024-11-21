using System;
using UnityEngine;

namespace Moonlander.Samples
{
	public class ThirdPersonInputs : MonoBehaviour
	{
		private Vector3 _previousMousePosition;
		
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		
		
		private void Update()
		{
			Vector2 moveDir = Vector2.zero;
			if (Input.GetKey(KeyCode.W))
				moveDir.y = 1;
			
			if (Input.GetKey(KeyCode.S))
				moveDir.y = -1;
			
			if (Input.GetKey(KeyCode.D))
				moveDir.x = 1;
			
			if (Input.GetKey(KeyCode.A))
				moveDir.x = -1;
			
			MoveInput(moveDir);

			// Look.
			LookInput(_previousMousePosition - Input.mousePosition);
			_previousMousePosition = Input.mousePosition;

			// Jump.
			JumpInput(Input.GetKey(KeyCode.Space));
			
			// Sprint.
			SprintInput(Input.GetKey(KeyCode.LeftShift));
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		public void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}
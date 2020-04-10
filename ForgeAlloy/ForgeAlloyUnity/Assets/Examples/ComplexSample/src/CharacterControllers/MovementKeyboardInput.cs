using UnityEngine;

namespace Forge.CharacterControllers
{
	public class MovementKeyboardInput : IMovementInputDevice
	{
		public Vector3 GetCurrentMoveDirection(Transform transform)
		{
			Vector3 direction = Vector3.zero;
			if (Input.GetKey(KeyCode.W))
				direction += transform.forward;
			if (Input.GetKey(KeyCode.S))
				direction -= transform.forward;
			if (Input.GetKey(KeyCode.A))
				direction -= transform.right;
			if (Input.GetKey(KeyCode.D))
				direction += transform.right;
			return direction.normalized;
		}

		public bool PerformedJump()
		{
			return Input.GetKeyDown(KeyCode.Space);
		}
	}
}
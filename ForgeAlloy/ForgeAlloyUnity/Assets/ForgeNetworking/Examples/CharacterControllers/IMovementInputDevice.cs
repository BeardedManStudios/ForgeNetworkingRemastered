using UnityEngine;

namespace Forge.CharacterControllers
{

	public interface IMovementInputDevice
	{
		bool PerformedJump();
		Vector3 GetCurrentMoveDirection(Transform transform);
	}
}
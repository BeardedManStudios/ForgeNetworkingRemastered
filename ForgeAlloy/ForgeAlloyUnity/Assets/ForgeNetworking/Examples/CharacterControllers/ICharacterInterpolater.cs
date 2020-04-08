using UnityEngine;

namespace Forge.CharacterControllers
{
	public interface ICharacterInterpolater
	{
		Transform Transform { get; }
		Transform CameraTransform { get; }
		void UpdateInterpolation(Vector3 position, Quaternion rotation, Quaternion camRotation);
	}
}

using UnityEngine;

namespace Forge.CharacterControllers
{
	public interface IRotationManager
	{
		float Speed { get; set; }
		void ProcessRotation(Transform host, Transform cameraTransform, Vector3 rotation);
	}
}

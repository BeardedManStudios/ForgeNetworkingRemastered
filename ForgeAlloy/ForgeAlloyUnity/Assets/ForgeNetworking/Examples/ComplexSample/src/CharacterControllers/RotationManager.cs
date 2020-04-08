using UnityEngine;

namespace Forge.CharacterControllers
{
	public class RotationManager : IRotationManager
	{
		private const float MIDPOINT = 180.0f;
		private const float MINIMUM = 271.0f;
		private const float MAXIMUM = 89.0f;

		public float Speed { get; set; } = 2.0f;

		public void ProcessRotation(Transform host, Transform cameraTransform, Vector3 rotation)
		{
			rotation *= Speed;
			host.eulerAngles += new Vector3(0.0f, rotation.y);
			float x = cameraTransform.eulerAngles.x - rotation.x;
			if (x > MIDPOINT)
				x = Mathf.Max(x, MINIMUM);
			else
				x = Mathf.Min(x, MAXIMUM);
			Vector3 rot = new Vector3(x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
			cameraTransform.eulerAngles = rot;
		}
	}
}

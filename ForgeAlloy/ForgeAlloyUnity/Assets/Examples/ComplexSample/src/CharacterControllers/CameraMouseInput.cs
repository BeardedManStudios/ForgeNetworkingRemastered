using UnityEngine;

namespace Forge.CharacterControllers
{
	public class CameraMouseInput : ICameraInputDevice
	{
		public Vector3 GetCurrentCameraRotation()
		{
			return new Vector3(Input.GetAxis("Mouse Y"),
				Input.GetAxis("Mouse X"), 0.0f);
		}
	}
}

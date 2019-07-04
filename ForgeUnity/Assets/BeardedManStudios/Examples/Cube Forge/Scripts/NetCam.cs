using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class NetCam : NetworkCameraBehavior
{
	/// <summary>
	/// The speed of the camera whenever it is moving
	/// </summary>
	public float speed = 5.0f;

	/// <summary>
	/// The reference to the camera component of this object
	/// </summary>
	private Camera cameraRef = null;

	/// <summary>
	/// Used to track the current players camera for LookAt rotation
	/// </summary>
	private static NetCam playerCamera = null;

	protected override void NetworkStart()
	{
		base.NetworkStart();

		cameraRef = GetComponent<Camera>();
		networkObject.UpdateInterval = 100;

		// If this is not our camera then we should not render using it
		if (!networkObject.IsOwner)
			cameraRef.enabled = false;
		else
			playerCamera = this;

		networkObject.position = transform.position;
		networkObject.SnapInterpolations();

		if (!networkObject.IsOwner)
			return;
	}

	private void Update()
	{
		if (networkObject == null)
			return;

		if (cameraRef == null)
			return;

		// If this is not owned by the current network client then it needs to
		// assign it to the position specified
		if (!networkObject.IsOwner)
		{
			transform.position = networkObject.position;

			if (playerCamera == null)
				return;

			// Make sure that this camera plane is always facing the current player
			transform.LookAt(transform.position + (transform.position - playerCamera.transform.position));
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			if (Physics.Raycast(cameraRef.ScreenPointToRay(Input.mousePosition), out hit))
			{
				var primitive = hit.transform.GetComponent<Primitive>();

				if (primitive == null)
					return;

				Vector3 position = hit.transform.position;
				position += hit.normal;

				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					if (!primitive.readOnly)
						CubeForgeGame.Instance.DestroyPrimitive(primitive);
				}
				else
					CubeForgeGame.Instance.CreatePrimitive(position);
			}
		}
		// If we right click we can move the camera as if we were in noclip mode
		else if (Input.GetMouseButton(1))
		{
			if (Input.GetKey(KeyCode.W))
				transform.position += transform.forward * speed * Time.deltaTime;
			if (Input.GetKey(KeyCode.S))
				transform.position -= transform.forward * speed * Time.deltaTime;
			if (Input.GetKey(KeyCode.A))
				transform.position -= transform.right * speed * Time.deltaTime;
			if (Input.GetKey(KeyCode.D))
				transform.position += transform.right * speed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
				transform.position += transform.up * speed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Z))
				transform.position -= transform.up * speed * Time.deltaTime;

			transform.Rotate(-Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

			Cursor.lockState = CursorLockMode.Locked | CursorLockMode.Confined;
			Cursor.visible = false;
		}
		else if (Input.GetMouseButtonUp(1))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		// The network object should always house the latest position of this camera
		networkObject.position = transform.position;
	}
}
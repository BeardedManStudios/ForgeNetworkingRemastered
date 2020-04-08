using Forge.Factory;
using UnityEngine;

namespace Forge.CharacterControllers.FirstPerson
{
	[RequireComponent(typeof(Rigidbody))]
	public class FirstPersonController : MonoBehaviour
	{
		[SerializeField] private float _speed = 0.0f;
		[SerializeField] private float _jumpSpeed = 0.0f;
		[SerializeField] private float _maxIncline = 0.0f;
		[SerializeField] private float _maxStepHeight = 0.0f;
		[SerializeField] private float _rotationSpeed = 0.0f;
		[SerializeField] private Transform _cameraTransform = null;
		[SerializeField] private Transform _friendPreviewTransform = null;
		[SerializeField] private RenderTexture _friendPreviewTexture = null;

		private IMovementInputDevice _moveInput;
		private ICameraInputDevice _cameraInput;
		private IVelocityManager _moveManager;
		private IRotationManager _rotationManager;
		private IFriendPreviewManager _friendPreviewManager;
		private INetworkTransformManager _networkTransform;

		private void Start()
		{
			ObtainInterfaces();
			InitMembers();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void ObtainInterfaces()
		{
			_moveInput = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<IMovementInputDevice>();
			_cameraInput = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<ICameraInputDevice>();
			_moveManager = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<IVelocityManager>();
			_rotationManager = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<IRotationManager>();

			// Network stuff
			_friendPreviewManager = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<IFriendPreviewManager>();
			_networkTransform = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<INetworkTransformManager>();
		}

		private void InitMembers()
		{
			_moveManager.Speed = _speed;
			_moveManager.JumpSpeed = _jumpSpeed;
			_moveManager.MaxIncline = _maxIncline;
			_moveManager.MaxStepHeight = _maxStepHeight;
			_rotationManager.Speed = _rotationSpeed;
			_moveManager.Initialize(transform);

			// Network stuff
			_networkTransform.Initialize(transform, _cameraTransform);
			_friendPreviewManager.Initialize(_friendPreviewTransform, _friendPreviewTexture);
		}

		private void Update()
		{
			if (_moveInput.PerformedJump())
				_moveManager.ProcessJump();
			_friendPreviewManager.Update();
			_networkTransform.Process();
		}

		private void FixedUpdate()
		{
			Vector3 moveDirection = _moveInput.GetCurrentMoveDirection(transform);
			Vector3 camearRotation = _cameraInput.GetCurrentCameraRotation();
			_rotationManager.ProcessRotation(transform, _cameraTransform, camearRotation);
			_moveManager.ProcessMovement(moveDirection);
		}

		private void OnCollisionEnter(Collision collision)
		{
			_moveManager.CollisionDeltas.Add(collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			_moveManager.CollisionDeltas.Remove(collision);
		}
	}
}
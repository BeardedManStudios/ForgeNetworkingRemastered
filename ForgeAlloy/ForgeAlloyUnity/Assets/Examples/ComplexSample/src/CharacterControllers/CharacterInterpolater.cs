using UnityEngine;

namespace Forge.CharacterControllers
{
	public class CharacterInterpolater : MonoBehaviour, ICharacterInterpolater
	{
		[SerializeField] private float _lerpT = 0.1f;
		[SerializeField] private float _sLerpT = 0.1f;
		[SerializeField] private Transform _cameraTransform = null;
		private Vector3 _targetPosition = Vector3.zero;
		private Quaternion _targetRotation = Quaternion.identity;
		private Quaternion _targetCamRotation = Quaternion.identity;

		public Transform Transform => transform;
		public Transform CameraTransform => _cameraTransform;

		private void Awake()
		{
			_targetPosition = transform.position;
			_targetRotation = transform.rotation;
		}

		private void Update()
		{
			transform.position = Vector3.Lerp(transform.position, _targetPosition, _lerpT);
			transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _sLerpT);
			_cameraTransform.localRotation = Quaternion.Slerp(_cameraTransform.localRotation, _targetCamRotation, _sLerpT);
		}

		public void UpdateInterpolation(Vector3 position, Quaternion rotation, Quaternion camRotation)
		{
			_targetPosition = position;
			_targetRotation = rotation;
			_targetCamRotation = camRotation;
		}
	}
}

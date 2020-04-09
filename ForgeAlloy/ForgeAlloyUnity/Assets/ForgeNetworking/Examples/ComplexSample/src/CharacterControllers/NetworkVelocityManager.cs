using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using Forge.Unity;
using Puzzle.Networking.Messages;
using UnityEngine;

namespace Forge.CharacterControllers
{
	public class NetworkTransformManager : INetworkTransformManager
	{
		private const float UPDATE_INTERVAL = 0.034f;

		private int _entityId = -1;
		private Transform _transform = null;
		private Transform _camTransform = null;
		private IEngineFacade _engine = null;
		private float _updateDelta = UPDATE_INTERVAL;
		private MessagePool<UpdatePlayerMessage> _msgPool = new MessagePool<UpdatePlayerMessage>();

		public void Initialize(Transform transform, Transform camTransform)
		{
			_transform = transform;
			_camTransform = camTransform;
			_engine = UnityExtensions.FindInterface<IEngineFacade>();
			if (_engine == null)
				throw new System.Exception("TODO:  The network engine could not be found");
			var e = _transform.GetComponent<IUnityEntity>();
			if (e == null)
				throw new System.Exception("TODO:  No network entity was found on this object");
			_entityId = e.Id;
		}

		public void Process()
		{
			_updateDelta -= Time.deltaTime;
			if (_updateDelta <= 0.0f)
			{
				_updateDelta = UPDATE_INTERVAL;
				var msg = _msgPool.Get();
				msg.EntityId = _entityId;
				msg.Position = _transform.position;
				msg.RotationY = _transform.eulerAngles.y;
				msg.RotationX = _camTransform.localEulerAngles.x;
				_engine.NetworkMediator.SendMessage(msg);
			}
		}
	}
}
using System.Net;
using Forge.CharacterControllers;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using UnityEngine;

namespace Puzzle.Networking.Messages.Interpreters
{
	public class UpdatePlayerInterpreter : IMessageInterpreter
	{
		public static UpdatePlayerInterpreter Instance { get; private set; } = new UpdatePlayerInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var m = (UpdatePlayerMessage)message;
			IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;
			try
			{
				IUnityEntity e = engine.EntityRepository.Get(m.EntityId);
				var interpolator = e.OwnerGameObject.GetComponent<ICharacterInterpolater>();
				Vector3 rot = e.OwnerGameObject.transform.eulerAngles;
				rot.y = m.RotationY;
				if (interpolator == null)
				{
					e.OwnerGameObject.transform.position = m.Position;
					e.OwnerGameObject.transform.eulerAngles = rot;
				}
				else
				{
					Vector3 camRot = interpolator.CameraTransform.localEulerAngles;
					camRot.x = m.RotationX;
					interpolator.UpdateInterpolation(m.Position, Quaternion.Euler(rot.x, rot.y, rot.z),
						Quaternion.Euler(camRot.x, camRot.y, camRot.z));
				}
			}
			catch (EntityNotFoundException ex)
			{
				// TODO:  This is getting called before the entity exists
				netMediator.EngineProxy.Logger.LogException(ex);
			}
		}
	}
}

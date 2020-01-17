using Forge.Networking.Messaging;
using System.Net;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class GetAllEntitiesRequestInterpreter : IMessageInterpreter
	{
		public static GetAllEntitiesRequestInterpreter Instance { get; private set; } = new GetAllEntitiesRequestInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var engine = (IEngineFacade)netMediator.EngineProxy;
			int count = engine.EntityRepository.Count;
			var msg = new SendEntitiesToNewPlayerMessage();
			msg.EntityCount = count;
			msg.Ids = new int[count];
			msg.PrefabIds = new int[count];
			msg.Positions = new Vector3[count];
			msg.Rotations = new Quaternion[count];
			msg.Scales = new Vector3[count];
			msg.SceneIdentifiers = new string[count];
			int i = 0;
			var itr = engine.EntityRepository.GetEnumerator();
			while (itr.MoveNext())
			{
				if (itr.Current != null)
				{
					var t = itr.Current.OwnerGameObject.transform;
					msg.Ids[i] = itr.Current.Id;
					msg.PrefabIds[i] = itr.Current.PrefabId;
					msg.Positions[i] = t.position;
					msg.Rotations[i] = t.rotation;
					msg.Scales[i] = t.localScale;
					msg.SceneIdentifiers[i] = itr.Current.SceneIdentifier;
				}
				i++;
			}
			netMediator.SendReliableMessage(msg, sender);
		}
	}
}

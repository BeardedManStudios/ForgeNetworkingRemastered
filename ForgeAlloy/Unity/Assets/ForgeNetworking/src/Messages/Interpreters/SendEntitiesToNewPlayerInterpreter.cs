using Forge.Networking.Messaging;
using System.Net;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public delegate void EntitiesLoadedEvent();

	public class SendEntitiesToNewPlayerInterpreter : IMessageInterpreter
	{
		public static SendEntitiesToNewPlayerInterpreter Instance { get; private set; } = new SendEntitiesToNewPlayerInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var sceneEntities = GameObject.FindObjectsOfType<NetworkEntity>() as IUnityEntity[];
			var m = (SendEntitiesToNewPlayerMessage)message;
			IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;
			for (int i = 0; i < m.EntityCount; i++)
			{
				if (string.IsNullOrEmpty(m.SceneIdentifiers[i]))
					EntitySpawnner.SpawnEntityFromData(engine, m.Ids[i], m.PrefabIds[i], m.Positions[i], m.Rotations[i], m.Scales[i]);
				else
					SetupEntityAlreadyInScene(sceneEntities, m, i, engine);
			}
			var msg = new CreateMyPlayerMessage();
			netMediator.SendReliableMessage(msg, sender);
		}

		private static void SetupEntityAlreadyInScene(IUnityEntity[] sceneEntities,
			SendEntitiesToNewPlayerMessage msg, int idx, IEngineFacade engine)
		{
			foreach (var e in sceneEntities)
			{
				if (e.SceneIdentifier == msg.SceneIdentifiers[idx])
				{
					e.Id = msg.Ids[idx];
					e.PrefabId = msg.PrefabIds[idx];
					Transform t = e.OwnerGameObject.transform;
					t.position = msg.Positions[idx];
					t.rotation = msg.Rotations[idx];
					t.localScale = msg.Scales[idx];
					engine.EntityRepository.Add(e);
				}
			}
		}
	}
}

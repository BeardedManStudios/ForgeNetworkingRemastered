using System.Net;
using Forge.Networking.Messaging;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SpawnPlayerRequestInterpreter : IMessageInterpreter
	{
		// TODO:  This should not be hard coded, it should be pulled from somewhere else
		private const int PLAYER_PREFAB_ID = 0;

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var engine = (IEngineFacade)netMediator.EngineProxy;
			var t = CreateGameObject(PLAYER_PREFAB_ID, engine);
			var e = t.GetComponent<IUnityEntity>();
			e.Id = engine.GetNewEntityId();
			// TODO:  Validate that the player has not already spawned
			var m = CreateMessage(t, e);
			AddMessageToNewClientBuffer(engine, m);
			SendMessageToAllClients(netMediator, m);
		}

		private static Transform CreateGameObject(int prefabId, IEngineFacade engine)
		{
			var p = engine.PrefabManager.GetPrefabById(prefabId);
			var t = GameObject.Instantiate(p);
			return t;
		}

		private static SpawnPlayerMessage CreateMessage(Transform t, IUnityEntity e)
		{
			var m = new SpawnPlayerMessage
			{
				PrefabId = PLAYER_PREFAB_ID,
				Position = t.position,
				Rotation = t.rotation,
				Scale = t.localScale,
				EntityId = e.Id
			};
			return m;
		}

		private static void AddMessageToNewClientBuffer(IEngineFacade engine, SpawnPlayerMessage m)
		{
			engine.NewClientMessageBuffer.AddMessage(m, engine.NetworkMediator.SocketFacade.ManagedSocket.EndPoint);
		}

		private static void SendMessageToAllClients(INetworkMediator netMediator, SpawnPlayerMessage m)
		{
			var itr = netMediator.PlayerRepository.GetEnumerator();
			while (itr.MoveNext())
			{
				if (itr.Current != null)
				{
					netMediator.MessageBus.SendReliableMessage(m,
						netMediator.SocketFacade.ManagedSocket, itr.Current.EndPoint);
				}
			}
		}
	}
}

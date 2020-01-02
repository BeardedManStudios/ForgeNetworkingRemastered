using System.Net;
using Forge.Networking.Messaging;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SpawnPlayerInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var m = (SpawnPlayerMessage)message;
			var engine = (IEngineFacade)netMediator.EngineProxy;
			var go = CreateGameObject(m, engine);
			SetupNetworkEntity(m, engine, go);
		}

		private static GameObject CreateGameObject(SpawnPlayerMessage m, IEngineFacade engine)
		{
			var p = engine.PrefabManager.GetPrefabById(m.PrefabId);
			var t = GameObject.Instantiate(p, m.Position, m.Rotation);
			t.localScale = m.Scale;
			return t.gameObject;
		}

		private static void SetupNetworkEntity(SpawnPlayerMessage m, IEngineFacade engine, GameObject go)
		{
			var e = go.GetComponent<IUnityEntity>();
			if (e == null)
			{
				// TODO:  Throw correct exception
				throw new System.Exception($"The prefab at index {m.PrefabId} in the Prefab manager " +
					$"should have a component that implements IUnityEntity but one was not found");
			}
			e.Id = m.EntityId;
			engine.EntityRepository.AddEntity(e);
		}
	}
}

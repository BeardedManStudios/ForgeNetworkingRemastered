using Forge.Networking.Unity.Messages;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public static class EntitySpawnner
	{
		public static void SpawnEntityFromMessage(IEngineFacade engine, SpawnEntityMessage message)
		{
			GameObject go = SpawnGameObject(message, engine);
			SetupNetworkEntity(message, engine, go);
		}

		private static GameObject SpawnGameObject(SpawnEntityMessage message, IEngineFacade engine)
		{
			var prefab = engine.PrefabManager.GetPrefabById(message.PrefabId);
			Transform t = GameObject.Instantiate(prefab, message.Position, message.Rotation);
			t.localScale = message.Scale;
			return t.gameObject;
		}

		private static void SetupNetworkEntity(SpawnEntityMessage message, IEngineFacade engine, GameObject go)
		{
			var entity = go.gameObject.AddComponent<NetworkEntity>();
			entity.Id = message.Id;
			engine.EntityRepository.Add(entity);
		}
	}
}

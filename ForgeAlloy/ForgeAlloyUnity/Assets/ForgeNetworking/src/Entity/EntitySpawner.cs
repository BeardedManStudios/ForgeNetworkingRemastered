using Forge.Networking.Unity.Messages;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public static class EntitySpawner
	{
		public static IUnityEntity SpawnEntityFromData(IEngineFacade engine, int id, int prefabId, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			var go = SpawnGameObjectWithInfo(engine, prefabId, pos, rot, scale);
			return SetupNetworkEntity(engine, go, id, prefabId);
		}

		public static IUnityEntity SpawnEntityFromMessage(IEngineFacade engine, SpawnEntityMessage message)
		{
			GameObject go = SpawnGameObject(message, engine);
			return SetupNetworkEntity(engine, go, message.Id, message.PrefabId);
		}

		private static GameObject SpawnGameObject(SpawnEntityMessage message, IEngineFacade engine)
		{
			return SpawnGameObjectWithInfo(engine, message.PrefabId, message.Position, message.Rotation, message.Scale);
		}

		private static GameObject SpawnGameObjectWithInfo(IEngineFacade engine, int prefabId, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			var prefab = engine.PrefabManager.GetPrefabById(prefabId);
			Transform t = GameObject.Instantiate(prefab, pos, rot);
			t.localScale = scale;
			return t.gameObject;
		}

		private static IUnityEntity SetupNetworkEntity(IEngineFacade engine, GameObject go, int id, int prefabId)
		{
			var entity = go.gameObject.AddComponent<NetworkEntity>();
			entity.Id = id;
			entity.PrefabId = prefabId;
			engine.EntityRepository.Add(entity);
			return entity;
		}
	}
}

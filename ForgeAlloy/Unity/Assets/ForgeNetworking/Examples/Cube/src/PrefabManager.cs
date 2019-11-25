using System.Collections.Generic;
using UnityEngine;

namespace Forge.Networking.Unity.Examples
{
	public class PrefabManager : MonoBehaviour, IPrefabManager
	{
		[SerializeField]
		private Transform[] _entities;

		private List<IUnityEntity> _registeredEntities;
		private void Awake()
		{
			for (int i = 0; i < _entities.Length; ++i)
			{
				var entity = _entities[i] as IUnityEntity;

				if (entity == null)
					continue;

				_registeredEntities.Add(entity);
			}

		}

		public IUnityEntity GetEntityById(int id)
		{
			IUnityEntity entity = default;

			for (int i = 0; i < _registeredEntities.Count; ++i)
			{
				if (_registeredEntities[i].CreationId == id)
				{
					entity = _registeredEntities[i];
					break;
				}
			}

			return entity;
		}
	}
}

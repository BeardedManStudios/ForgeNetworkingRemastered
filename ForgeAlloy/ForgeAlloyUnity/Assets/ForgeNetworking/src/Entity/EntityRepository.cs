using System.Collections.Generic;

namespace Forge.Networking.Unity
{
	public class EntityRepository : IEntityRepository
	{
		public int Count => _entities.Count;
		private readonly Dictionary<int, IUnityEntity> _entities = new Dictionary<int, IUnityEntity>();

		public void Add(IUnityEntity entity)
		{
			if (_entities.ContainsKey(entity.Id))
				throw new EntityAlreadyInRepositoryException(entity.Id);
			_entities.Add(entity.Id, entity);
		}

		public IUnityEntity Get(int id)
		{
			if (_entities.TryGetValue(id, out var e))
				return e;
			throw new EntityNotFoundException(id);
		}

		public void Remove(int id)
		{
			_entities.Remove(id);
		}

		public void Remove(IUnityEntity entity)
		{
			_entities.Remove(entity.Id);
		}

		public IEnumerator<IUnityEntity> GetEnumerator()
		{
			return _entities.Values.GetEnumerator();
		}

		public bool Exists(int id)
		{
			return _entities.ContainsKey(id);
		}

		public bool Exists(IUnityEntity e)
		{
			foreach (var kv in _entities)
				if (kv.Value == e)
					return true;
			return false;
		}
	}
}

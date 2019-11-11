using System.Collections.Generic;

namespace Forge.Engine
{
	public class ForgeEntityRepository : IEntityRepository
	{
		private Dictionary<int, IEntity> _entityLookup = new Dictionary<int, IEntity>();

		public event EntityAddedToRepository onEntityAdded;

		public void AddEntity(IEntity entity)
		{
			if (_entityLookup.ContainsKey(entity.Id))
				throw new EntityExistsInRepositoryException(entity.Id);
			_entityLookup.Add(entity.Id, entity);
			onEntityAdded(entity);
		}

		public IEntity GetEntityById(int id)
		{
			if (!_entityLookup.TryGetValue(id, out var entity))
				throw new EntityNotInRepositoryException(id);
			return entity;
		}

		public bool HasEntity(int id)
		{
			return _entityLookup.ContainsKey(id);
		}

		public void RemoveEntity(IEntity entity)
		{
			_entityLookup.Remove(entity.Id);
		}

		public void RemoveEntity(int id)
		{
			_entityLookup.Remove(id);
		}
	}
}

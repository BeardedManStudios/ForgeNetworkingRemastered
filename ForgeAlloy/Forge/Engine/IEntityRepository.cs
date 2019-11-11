namespace Forge.Engine
{
	public delegate void EntityAddedToRepository(IEntity entity);

	public interface IEntityRepository
	{
		event EntityAddedToRepository onEntityAdded;
		void AddEntity(IEntity entity);
		IEntity GetEntityById(int id);
		void RemoveEntity(IEntity entity);
		void RemoveEntity(int id);
		bool HasEntity(int id);
	}
}

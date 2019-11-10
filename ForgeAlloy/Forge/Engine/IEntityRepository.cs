namespace Forge.Engine
{
	public interface IEntityRepository
	{
		void AddEntity(IEntity entity);
		IEntity GetEntityById(int id);
		void RemoveEntity(IEntity entity);
		void RemoveEntity(int id);
		bool HasEntity(int id);
	}
}

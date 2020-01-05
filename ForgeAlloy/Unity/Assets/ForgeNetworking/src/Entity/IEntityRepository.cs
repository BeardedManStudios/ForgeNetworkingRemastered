namespace Forge.Networking.Unity
{
	public interface IEntityRepository
	{
		void Add(IUnityEntity entity);
		IUnityEntity Get(int id);
		void Remove(int id);
		void Remove(IUnityEntity entity);
	}
}

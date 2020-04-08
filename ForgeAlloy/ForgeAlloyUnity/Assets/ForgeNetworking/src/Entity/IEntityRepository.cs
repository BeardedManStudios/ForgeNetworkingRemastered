using System.Collections.Generic;

namespace Forge.Networking.Unity
{
	public interface IEntityRepository
	{
		int Count { get; }
		void Add(IUnityEntity entity);
		IUnityEntity Get(int id);
		void Remove(int id);
		void Remove(IUnityEntity entity);
		IEnumerator<IUnityEntity> GetEnumerator();
		bool Exists(int id);
		bool Exists(IUnityEntity e);
	}
}

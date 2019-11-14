namespace Forge.DataStructures.Repository
{
	public delegate void RepositoryChanged<T>(T obj);
	public interface IRepository<TKey, TObject>
	{
		event RepositoryChanged<TObject> onAdd;
		event RepositoryChanged<TObject> onRemove;
		void Add(TKey key, TObject obj);
		void Remove(TKey key);
		bool Exists(TKey key);
	}
}

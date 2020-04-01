using System.Collections.Generic;

namespace Forge.Repository
{
	public class KeyedRepository<TKey, TObject> : IRepository<TKey, TObject>
	{
		public event RepositoryChanged<TObject> onAdd;
		public event RepositoryChanged<TObject> onRemove;

		protected readonly Dictionary<TKey, TObject> dict = new Dictionary<TKey, TObject>();

		public virtual void Add(TKey key, TObject obj)
		{
			if (dict.ContainsKey(key))
			{
				throw new KeyNotFoundException($"The key {key} could not" +
					$" be located in the repository");
			}
			dict.Add(key, obj);
			onAdd?.Invoke(obj);
		}

		public virtual bool Exists(TKey key)
		{
			return dict.ContainsKey(key);
		}

		public virtual void Remove(TKey key)
		{
			if (dict.TryGetValue(key, out var obj))
			{
				dict.Remove(key);
				onRemove?.Invoke(obj);
			}
		}
	}
}

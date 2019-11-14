using System.Collections.Generic;

namespace Forge.DataStructures.Repository
{
	public class KeyedRepository<TKey, TObject> : IRepository<TKey, TObject>
	{
		public event RepositoryChanged<TObject> onAdd;
		public event RepositoryChanged<TObject> onRemove;

		private readonly Dictionary<TKey, TObject> _dict = new Dictionary<TKey, TObject>();

		public void Add(TKey key, TObject obj)
		{
			if (_dict.ContainsKey(key))
			{
				throw new KeyNotFoundException($"The key {key} could not" +
					$" be located in the repository");
			}
			_dict.Add(key, obj);
			onAdd?.Invoke(obj);
		}

		public bool Exists(TKey key)
		{
			return _dict.ContainsKey(key);
		}

		public void Remove(TKey key)
		{
			if (_dict.TryGetValue(key, out var obj))
			{
				_dict.Remove(key);
				onRemove?.Invoke(obj);
			}
		}
	}
}

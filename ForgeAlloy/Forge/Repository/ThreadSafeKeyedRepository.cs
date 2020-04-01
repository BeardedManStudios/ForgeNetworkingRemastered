using System.Collections.Generic;

namespace Forge.Repository
{
	public sealed class ThreadSafeKeyedRepository<TKey, TObject> : IRepository<TKey, TObject>
	{
		public event RepositoryChanged<TObject> onAdd;
		public event RepositoryChanged<TObject> onRemove;

		private readonly Dictionary<TKey, TObject> _dict = new Dictionary<TKey, TObject>();

		public void Add(TKey key, TObject obj)
		{
			lock (_dict)
			{
				if (_dict.ContainsKey(key))
				{
					throw new KeyNotFoundException($"The key {key} could not" +
						$" be located in the repository");
				}
				_dict.Add(key, obj);
				onAdd?.Invoke(obj);
			}
		}

		public bool Exists(TKey key)
		{
			lock (_dict)
			{
				return _dict.ContainsKey(key);
			}
		}

		public void Remove(TKey key)
		{
			lock (_dict)
			{
				if (_dict.TryGetValue(key, out var obj))
				{
					_dict.Remove(key);
					onRemove?.Invoke(obj);
				}
			}
		}
	}
}

using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public class MessagePool<T> where T : IMessage, new()
	{
		private readonly List<T> _poolAvailable = new List<T>();
		private readonly List<T> _poolInUse = new List<T>();

		public T Get()
		{
			if (_poolAvailable.Count == 0)
				return CreateNewPoolEntry();
			else
				return GetAvailable();
		}

		private T CreateNewPoolEntry()
		{
			var m = new T();
			m.OnMessageSent += Release;
			_poolInUse.Add(m);
			return m;
		}

		private T GetAvailable()
		{
			var m = _poolAvailable[0];
			_poolAvailable.RemoveAt(0);
			_poolInUse.Add(m);
			return m;
		}

		private void Release(IMessage message)
		{
			var m = (T)message;
			_poolInUse.Remove(m);
			_poolAvailable.Add(m);
		}
	}
}

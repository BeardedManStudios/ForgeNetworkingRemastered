using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public class MessagePool<T> where T : IMessage, new()
	{
		private readonly Queue<T> _poolAvailable = new Queue<T>();

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
			return m;
		}

		private T GetAvailable()
		{
			var m = _poolAvailable.Dequeue();
			return m;
		}

		private void Release(IMessage message)
		{
			var m = (T)message;
			_poolAvailable.Enqueue(m);
		}
	}
}

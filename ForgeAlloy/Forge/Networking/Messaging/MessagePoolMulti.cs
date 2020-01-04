using System;
using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public class MessagePoolMulti
	{
		private class Pool
		{
			public List<IMessage> Available = new List<IMessage>();
			public List<IMessage> InUse = new List<IMessage>();
		}

		private readonly Dictionary<Type, Pool> _messagePools = new Dictionary<Type, Pool>();

		public IMessage Get(Type t)
		{
			var pool = GetPool(t);
			if (pool.Available.Count == 0)
				return CreateNewMessageForPool(t, pool);
			else
				return GetAvailable(pool);
		}

		public T Get<T>() where T : IMessage, new()
		{
			var pool = GetPool(typeof(T));
			if (pool.Available.Count == 0)
				return CreateNewMessageForPool<T>(pool);
			else
				return (T)GetAvailable(pool);
		}

		private Pool GetPool(Type type)
		{
			if (!_messagePools.TryGetValue(type, out var pool))
			{
				pool = new Pool();
				_messagePools.Add(type, pool);
			}
			return pool;
		}

		private T CreateNewMessageForPool<T>(Pool pool) where T : IMessage, new()
		{
			T m = new T();
			m.OnMessageSent += Release;
			pool.InUse.Add(m);
			return m;
		}

		private IMessage CreateNewMessageForPool(Type t, Pool pool)
		{
			IMessage m = (IMessage)Activator.CreateInstance(t);
			m.OnMessageSent += Release;
			pool.InUse.Add(m);
			return m;
		}

		private void Release(IMessage message)
		{
			Pool pool = GetPool(message.GetType());
			pool.InUse.Remove(message);
			pool.Available.Add(message);
		}

		private IMessage GetAvailable(Pool pool)
		{
			var m = pool.Available[0];
			pool.Available.RemoveAt(0);
			pool.InUse.Add(m);
			return m;
		}
	}
}

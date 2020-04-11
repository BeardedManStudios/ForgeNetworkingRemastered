using System;
using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public class MessagePoolMulti
	{
		private readonly Dictionary<Type, Queue<IMessage>> _messagePools = new Dictionary<Type, Queue<IMessage>>();

		public IMessage Get(Type t)
		{
			var pool = GetPool(t);
			if (pool.Count == 0)
				return CreateNewMessageForPool(t, pool);
			else
				return pool.Dequeue();
		}

		public T Get<T>() where T : IMessage, new()
		{
			var pool = GetPool(typeof(T));
			if (pool.Count == 0)
				return CreateNewMessageForPool<T>(pool);
			else
				return (T)pool.Dequeue();
		}

		private Queue<IMessage> GetPool(Type type)
		{
			if (!_messagePools.TryGetValue(type, out var pool))
			{
				pool = new Queue<IMessage>();
				_messagePools.Add(type, pool);
			}
			return pool;
		}

		private T CreateNewMessageForPool<T>(Queue<IMessage> pool) where T : IMessage, new()
		{
			T m = new T();
			m.OnMessageSent += Release;
			return m;
		}

		private IMessage CreateNewMessageForPool(Type t, Queue<IMessage> pool)
		{
			IMessage m = (IMessage)Activator.CreateInstance(t);
			m.OnMessageSent += Release;
			return m;
		}

		private void Release(IMessage message)
		{
			Queue<IMessage> pool = GetPool(message.GetType());
			pool.Enqueue(message);
		}
	}
}

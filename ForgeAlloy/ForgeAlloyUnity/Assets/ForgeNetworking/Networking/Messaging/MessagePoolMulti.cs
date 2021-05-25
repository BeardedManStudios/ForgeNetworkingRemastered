using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Forge.Networking.Messaging
{
	public class MessagePoolMulti
	{
		private readonly Dictionary<Type, ConcurrentQueue<IMessage>> _messagePools = new Dictionary<Type, ConcurrentQueue<IMessage>>();

		public IMessage Get(Type t)
		{
			var pool = GetPool(t);
			if (pool.Count == 0)
				return CreateNewMessageForPool(t, pool);
			else
			{
				// Try to dequeue, but if locked default to create new
				IMessage item;
				if (pool.TryDequeue(out item))
				{
					item.IsPooled = false;
					return item;
				}
				else return CreateNewMessageForPool(t, pool);
			}
		}

		public T Get<T>() where T : IMessage, new()
		{
			var pool = GetPool(typeof(T));
			if (pool.Count == 0)
				return CreateNewMessageForPool<T>(pool);
			else
			{
				// Try to dequeue, but if locked default to create new
				IMessage item;
				if (pool.TryDequeue(out item))
				{
					item.IsPooled = false;
					return (T)item;
				}
				else return CreateNewMessageForPool<T>(pool);
			}
		}

		private ConcurrentQueue<IMessage> GetPool(Type type)
		{
			if (!_messagePools.TryGetValue(type, out var pool))
			{
				pool = new ConcurrentQueue<IMessage>();
				_messagePools.Add(type, pool);
			}
			return pool;
		}

		private T CreateNewMessageForPool<T>(ConcurrentQueue<IMessage> pool) where T : IMessage, new()
		{
			T m = new T();
			m.OnMessageSent += Release;
			return m;
		}

		private IMessage CreateNewMessageForPool(Type t, ConcurrentQueue<IMessage> pool)
		{
			IMessage m = (IMessage)Activator.CreateInstance(t);
			m.OnMessageSent += Release;
			return m;
		}

		private void Release(IMessage message)
		{
			if (message.IsPooled) return; // Message has already been returned to pool
			if (!message.IsSent) return;	// Message has not been sent, not ready to return to pool
			if (message.IsBuffered) return; // Message is still buffered, not ready to return to pool
			message.IsSent = false;
			message.IsPooled = true;
			message.Receipt = null;
			ConcurrentQueue<IMessage> pool = GetPool(message.GetType());
			pool.Enqueue(message);
		}
	}
}

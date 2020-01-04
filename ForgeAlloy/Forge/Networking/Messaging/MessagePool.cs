using System;
using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public class MessagePool
	{
		private class PoolEntry
		{
			public bool Available { get; set; }
			public IMessage Message { get; set; }
		}

		private Dictionary<Type, List<PoolEntry>> _messagePools = new Dictionary<Type, List<PoolEntry>>();

		public IMessage Get<T>() where T : IMessage, new()
		{
			var pool = GetPool(typeof(T));
			for (int i = 0; i < pool.Count; i++)
			{
				if (pool[i].Available)
				{
					pool[i].Available = false;
					return pool[i].Message;
				}
			}
			return CreateNewMessageForPool<T>(pool);
		}

		private void Release(IMessage message)
		{
			List<PoolEntry> pool = GetPool(message.GetType());
			for (int i = 0; i < pool.Count; i++)
			{
				if (pool[i].Message == message)
				{
					pool[i].Available = true;
					break;
				}
			}
		}

		private List<PoolEntry> GetPool(Type type)
		{
			if (!_messagePools.TryGetValue(type, out var pool))
			{
				pool = new List<PoolEntry>();
				_messagePools.Add(type, pool);
			}
			return pool;
		}

		private IMessage CreateNewMessageForPool<T>(List<PoolEntry> pool) where T : IMessage, new()
		{
			IMessage m = new T();
			m.OnMessageSent += Release;
			pool.Add(new PoolEntry
			{
				Available = false,
				Message = m
			});
			return m;
		}
	}
}

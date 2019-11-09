using System;

namespace Forge.Networking.Messaging
{
	public interface IMessageRepository
	{
		void AddMessage(IMessage message);
		void AddMessage(IMessage message, int ttlMilliseconds);
		void RemoveMessage(IMessage message);
		void RemoveMessage(Guid guid);
		bool Exists(Guid guid);
		void Clear();
	}
}

using System.Collections.Generic;
using System.Net;

namespace Forge.Networking.Messaging
{
	public delegate void MessageRepositoryIterator(EndPoint endpoint, IMessage message);

	public interface IMessageRepository
	{
		void AddMessage(IMessage message, EndPoint sender);
		void AddMessage(IMessage message, EndPoint sender, int ttlMilliseconds);
		void RemoveAllFor(EndPoint sender);
		void RemoveMessage(IMessage message);
		void RemoveMessage(IMessageReceiptSignature receipt);
		bool Exists(IMessageReceiptSignature receipt);
		KeyValuePair<EndPoint, IMessage> Get(IMessageReceiptSignature receipt);
		void Iterate(MessageRepositoryIterator iterator);
		void Clear();
	}
}

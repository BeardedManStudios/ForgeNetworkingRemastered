using System.Collections.Generic;
using System.Net;

namespace Forge.Networking.Messaging
{
	public interface IMessageRepository
	{
		void AddMessage(IMessage message, EndPoint sender);
		void AddMessage(IMessage message, EndPoint sender, int ttlMilliseconds);
		void RemoveAllFor(EndPoint sender);
		void RemoveMessage(IMessage message);
		void RemoveMessage(IMessageReceipt receipt);
		bool Exists(IMessageReceipt receipt);
		KeyValuePair<EndPoint, IMessage> Get(IMessageReceipt receipt);
		Dictionary<IMessageReceipt, KeyValuePair<EndPoint, IMessage>>.ValueCollection GetIterator();
		void Clear();
	}
}

using System;
using System.Collections.Generic;
using System.Net;

namespace Forge.Networking.Messaging
{
	public interface IMessageRepository
	{
		void AddMessage(IMessage message, EndPoint sender);
		void AddMessage(IMessage message, EndPoint sender, int ttlMilliseconds);
		void RemoveMessage(IMessage message);
		void RemoveMessage(Guid guid);
		bool Exists(Guid guid);
		KeyValuePair<EndPoint, IMessage> Get(Guid guid);
		Dictionary<Guid, KeyValuePair<EndPoint, IMessage>>.ValueCollection GetIterator();
		void Clear();
		// TODO:  Will need a way to either group messages or get all of them to re-send
	}
}

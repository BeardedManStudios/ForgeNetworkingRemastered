using System.Net;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging
{
	public interface IMessageBus
	{
		IMessageBufferInterpreter MessageBufferInterpreter { get; }
		void SendMessage(IMessage message, ISocket sender, EndPoint reciever);
		IMessageReceipt SendReliableMessage(IMessage message, ISocket sender, EndPoint reciever);
		void ReceiveMessageBuffer(ISocket readingSocket, EndPoint messageSender, byte[] messageBuffer);
		void SetMediator(INetworkMediator mediator);
	}
}

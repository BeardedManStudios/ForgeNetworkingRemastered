using System.Net;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public interface IMessageBus
	{
		IMessageBufferInterpreter MessageBufferInterpreter { get; }
		void SendMessage(IMessage message, ISocket sender, EndPoint reciever);
		IMessageReceiptSignature SendReliableMessage(IMessage message, ISocket sender, EndPoint receiver);
		void ReceiveMessageBuffer(ISocket readingSocket, EndPoint messageSender, BMSByte messageBuffer);
		void SetMediator(INetworkMediator mediator);
		void MessageConfirmed(EndPoint sender, IMessageReceiptSignature messageReceipt);
	}
}

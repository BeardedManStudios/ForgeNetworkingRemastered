using Forge.Networking.Messaging.Paging;
using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging
{
	public interface IMessageBus
	{
		IMessageBufferInterpreter MessageBufferInterpreter { get; }
		void SendMessage(IMessage message, ISocket sender, ISocket reciever);
		IMessageReceipt SendReliableMessage(IMessage message, ISocket sender, ISocket reciever);
		void ReceiveMessageBuffer(INetworkContainer host, ISocket readingSocket, ISocket messageSender, byte[] messageBuffer);
	}
}

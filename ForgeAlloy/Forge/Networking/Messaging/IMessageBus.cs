using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging
{
	public interface IMessageBus
	{
		void SendMessage(IMessage message, ISocket reciever);
		IMessageReceipt SendReliableMessage(IMessage message, ISocket reciever);
		void ReceiveMessageBuffer(INetworkContainer host, ISocket sender, byte[] messageBuffer);
	}
}

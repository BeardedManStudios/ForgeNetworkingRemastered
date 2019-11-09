namespace Forge.Networking.Messaging
{
	public interface IMessageBus
	{
		void SendMessage(IMessage message, IMessageReciever reciever);
		IMessageReceipt SendReliableMessage(IMessage message, IMessageReciever reciever);
		void ReceiveMessageBuffer(INetworkHost host, byte[] messageBuffer);
	}
}

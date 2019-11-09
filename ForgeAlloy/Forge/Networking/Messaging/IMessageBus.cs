namespace Forge.Networking.Messaging
{
	public interface IMessageBus
	{
		void SendMessage(IMessage message, IMessageClient reciever);
		IMessageReceipt SendReliableMessage(IMessage message, IMessageClient reciever);
		void ReceiveMessageBuffer(INetwork host, IMessageClient sender, byte[] messageBuffer);
	}
}

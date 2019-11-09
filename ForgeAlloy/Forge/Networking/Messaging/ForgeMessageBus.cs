using System;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageBus : IMessageBus
	{
		public void SendMessage(IMessage message, IMessageReciever reciever)
		{
			reciever.Send(message);
		}

		public IMessageReceipt SendReliableMessage(IMessage message, IMessageReciever reciever)
		{
			var receipt = ForgeTypeFactory.Get<IMessageReceipt>();
			receipt.Signature = Guid.NewGuid();
			message.Receipt = receipt;
			reciever.Send(message);
			return receipt;
		}
	}
}

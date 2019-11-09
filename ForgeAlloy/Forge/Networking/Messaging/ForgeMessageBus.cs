using System;
using Forge.Networking.Messaging.Messages;
using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageBus : IMessageBus
	{
		public void SendMessage(IMessage message, IMessageClient receiver)
		{
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			ObjectMapper.Instance.MapBytes(buffer, message.MessageCode, message.Receipt?.Signature.ToString() ?? "");
			message.Serialize(buffer);
			receiver.Send(buffer.CompressBytes());
		}

		public IMessageReceipt SendReliableMessage(IMessage message, IMessageClient receiver)
		{
			var receipt = ForgeTypeFactory.Get<IMessageReceipt>();
			receipt.Signature = Guid.NewGuid();
			message.Receipt = receipt;
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			ObjectMapper.Instance.MapBytes(buffer, message.MessageCode, message.Receipt?.Signature.ToString() ?? "");
			message.Serialize(buffer);
			receiver.Send(buffer.CompressBytes());
			return receipt;
		}

		public void ReceiveMessageBuffer(INetwork host, IMessageClient sender, byte[] messageBuffer)
		{
			var buffer = new BMSByte();
			buffer.Clone(messageBuffer);
			var m = CreateMessageTypeFromBuffer(buffer);
			ProcessBufferGuid(sender, buffer, m);
			m.Deserialize(buffer);
			m.Interpret(host);
		}

		private static IMessage CreateMessageTypeFromBuffer(BMSByte buffer)
		{
			int code = buffer.GetBasicType<int>();
			var m = (IMessage)ForgeMessageCodes.Instantiate(code);
			m.MessageCode = code;
			return m;
		}

		private void ProcessBufferGuid(IMessageClient sender, BMSByte buffer, IMessage m)
		{
			string guid = buffer.GetBasicType<string>();
			if (guid.Length == 0)
				return;
			m.Receipt = ForgeTypeFactory.Get<IMessageReceipt>();
			m.Receipt.Signature = Guid.Parse(guid);
			SendMessage(new ForgeReceiptAcknowledgement { ReceiptGuid = guid }, sender);
		}
	}
}

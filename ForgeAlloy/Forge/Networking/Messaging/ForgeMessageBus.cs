using System;
using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageBus : IMessageBus
	{
		public void SendMessage(IMessage message, IMessageReciever reciever)
		{
			var buffer = new BMSByte();
			buffer.SetSize(128);
			ObjectMapper.Instance.MapBytes(buffer, message.MessageCode, message.Receipt?.Signature.ToString() ?? "");
			message.Serialize(buffer);
			reciever.Send(buffer.CompressBytes());
		}

		public IMessageReceipt SendReliableMessage(IMessage message, IMessageReciever reciever)
		{
			var receipt = ForgeTypeFactory.Get<IMessageReceipt>();
			receipt.Signature = Guid.NewGuid();
			message.Receipt = receipt;
			var buffer = new BMSByte();
			buffer.SetSize(128);
			ObjectMapper.Instance.MapBytes(buffer, message.MessageCode, message.Receipt?.Signature.ToString() ?? "");
			message.Serialize(buffer);
			reciever.Send(buffer.CompressBytes());
			return receipt;
		}

		public void ReceiveMessageBuffer(INetworkHost host, byte[] messageBuffer)
		{
			var buffer = new BMSByte();
			buffer.Clone(messageBuffer);
			int code = buffer.GetBasicType<int>();
			var m = (IMessage)ForgeMessageCodes.Instantiate(code);
			m.MessageCode = code;
			string guid = buffer.GetBasicType<string>();
			if (guid.Length > 0)
			{
				m.Receipt = new ForgeMessageReceipt
				{
					Signature = Guid.Parse(guid)
				};
			}
			m.Deserialize(buffer);
			m.Interpret(host);
		}
	}
}

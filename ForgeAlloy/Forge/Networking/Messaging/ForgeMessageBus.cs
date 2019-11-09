﻿using System;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageBus : IMessageBus
	{
		private static int GetMessageCode(IMessage message)
		{
			return ForgeMessageCodes.GetCodeFromType(message.GetType());
		}

		public void SendMessage(IMessage message, ISocket receiver)
		{
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			ObjectMapper.Instance.MapBytes(buffer, GetMessageCode(message), message.Receipt?.Signature.ToString() ?? "");
			message.Serialize(buffer);
			byte[] messageBuffer = buffer.CompressBytes();
			receiver.Send(messageBuffer, messageBuffer.Length);
		}

		public IMessageReceipt SendReliableMessage(IMessage message, ISocket receiver)
		{
			var receipt = ForgeTypeFactory.Get<IMessageReceipt>();
			receipt.Signature = Guid.NewGuid();
			message.Receipt = receipt;
			var buffer = new BMSByte();
			buffer.SetArraySize(128);

			ObjectMapper.Instance.MapBytes(buffer, GetMessageCode(message), message.Receipt?.Signature.ToString() ?? "");
			message.Serialize(buffer);
			byte[] messageBuffer = buffer.CompressBytes();
			receiver.Send(messageBuffer, messageBuffer.Length);
			return receipt;
		}

		public void ReceiveMessageBuffer(INetworkContainer host, ISocket sender, byte[] messageBuffer)
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
			return (IMessage)ForgeMessageCodes.Instantiate(code);
		}

		private void ProcessBufferGuid(ISocket sender, BMSByte buffer, IMessage m)
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
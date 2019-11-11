using System;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageBus : IMessageBus
	{
		public IMessageBufferInterpreter MessageBufferInterpreter { get; private set; }
		private readonly IMessageDestructor _messageDestructor;

		public ForgeMessageBus()
		{
			MessageBufferInterpreter = ForgeTypeFactory.GetNew<IMessageBufferInterpreter>();
			_messageDestructor = ForgeTypeFactory.GetNew<IMessageDestructor>();
		}

		private static int GetMessageCode(IMessage message)
		{
			return ForgeMessageCodes.GetCodeFromType(message.GetType());
		}

		public void SendMessage(IMessage message, ISocket sender, ISocket receiver)
		{
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			buffer.Append(
				ForgeSerializationContainer.Instance.Serialize(GetMessageCode(message)),
				ForgeSerializationContainer.Instance.Serialize(message.Receipt?.Signature.ToString() ?? "")
			);
			message.Serialize(buffer);
			IPagenatedMessage pm = _messageDestructor.BreakdownMessage(buffer);
			byte[] messageBuffer = pm.Buffer.CompressBytes();
			sender.Send(receiver, messageBuffer, messageBuffer.Length);
		}

		public IMessageReceipt SendReliableMessage(IMessage message, ISocket sender, ISocket receiver)
		{
			var receipt = ForgeTypeFactory.GetNew<IMessageReceipt>();
			receipt.Signature = Guid.NewGuid();
			message.Receipt = receipt;
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			buffer.Append(
				ForgeSerializationContainer.Instance.Serialize(GetMessageCode(message)),
				ForgeSerializationContainer.Instance.Serialize(message.Receipt?.Signature.ToString() ?? "")
			);
			message.Serialize(buffer);
			IPagenatedMessage pm = _messageDestructor.BreakdownMessage(buffer);
			byte[] messageBuffer = pm.Buffer.CompressBytes();
			sender.Send(receiver, messageBuffer, messageBuffer.Length);
			return receipt;
		}

		public void ReceiveMessageBuffer(INetworkContainer host, ISocket readingSocket, ISocket messageSender, byte[] messageBuffer)
		{
			var buffer = new BMSByte();
			buffer.Clone(messageBuffer);
			IMessageConstructor constructor = MessageBufferInterpreter.ReconstructPacketPage(buffer);
			if (constructor.MessageReconstructed)
			{
				var m = CreateMessageTypeFromBuffer(constructor.MessageBuffer);
				ProcessMessageSignature(readingSocket, messageSender, constructor.MessageBuffer, m);
				m.Deserialize(constructor.MessageBuffer);
				m.Interpret(host);
			}
		}

		private static IMessage CreateMessageTypeFromBuffer(BMSByte buffer)
		{
			int code = buffer.GetBasicType<int>();
			return (IMessage)ForgeMessageCodes.Instantiate(code);
		}

		private void ProcessMessageSignature(ISocket readingSocket, ISocket messageSender, BMSByte buffer, IMessage m)
		{
			string guid = buffer.GetBasicType<string>();
			if (guid.Length == 0)
				return;
			m.Receipt = ForgeTypeFactory.GetNew<IMessageReceipt>();
			m.Receipt.Signature = Guid.Parse(guid);
			SendMessage(new ForgeReceiptAcknowledgement { ReceiptGuid = guid }, readingSocket, messageSender);
		}
	}
}

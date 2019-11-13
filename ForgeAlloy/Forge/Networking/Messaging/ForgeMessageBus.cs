using System;
using System.Net;
using Forge.Factory;
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
			MessageBufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			_messageDestructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
		}

		private static int GetMessageCode(IMessage message)
		{
			return ForgeMessageCodes.GetCodeFromType(message.GetType());
		}

		public void SendMessage(IMessage message, ISocket sender, EndPoint receiver)
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

		public IMessageReceipt SendReliableMessage(IMessage message, ISocket sender, EndPoint receiver)
		{
			var receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceipt>();
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

		public void ReceiveMessageBuffer(INetworkContainer netContainer, ISocket readingSocket, EndPoint messageSender, byte[] messageBuffer)
		{
			var buffer = new BMSByte();
			buffer.Clone(messageBuffer);
			IMessageConstructor constructor = MessageBufferInterpreter.ReconstructPacketPage(buffer);
			if (constructor.MessageReconstructed)
			{
				var m = CreateMessageTypeFromBuffer(constructor.MessageBuffer);
				ProcessMessageSignature(readingSocket, messageSender, constructor.MessageBuffer, m);
				m.Deserialize(constructor.MessageBuffer);

				// TODO:  I don't like this type check and if branching in here...
				bool isServer = netContainer.SocketContainer is ISocketServerContainer;

				var interpreter = m.Interpreter;
				if (interpreter.ValidOnClient && !isServer)
					interpreter.Interpret(netContainer, messageSender, m);
				else if (interpreter.ValidOnServer && isServer)
					interpreter.Interpret(netContainer, messageSender, m);
			}
		}

		private static IMessage CreateMessageTypeFromBuffer(BMSByte buffer)
		{
			int code = buffer.GetBasicType<int>();
			return (IMessage)ForgeMessageCodes.Instantiate(code);
		}

		private void ProcessMessageSignature(ISocket readingSocket, EndPoint messageSender, BMSByte buffer, IMessage m)
		{
			string guid = buffer.GetBasicType<string>();
			if (guid.Length == 0)
				return;
			m.Receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceipt>();
			m.Receipt.Signature = Guid.Parse(guid);
			SendMessage(new ForgeReceiptAcknowledgement { ReceiptGuid = guid }, readingSocket, messageSender);
		}
	}
}

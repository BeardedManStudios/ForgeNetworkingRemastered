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
		private IMessageRepeater _messageRepeater;
		public IMessageBufferInterpreter MessageBufferInterpreter { get; private set; }
		private readonly IMessageDestructor _messageDestructor;
		private INetworkMediator _networkMediator;

		public ForgeMessageBus()
		{
			MessageBufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			_messageDestructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
		}

		public void SetMediator(INetworkMediator networkMediator)
		{
			_networkMediator = networkMediator;
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
				ForgeSerializationStrategy.Instance.Serialize(GetMessageCode(message)),
				ForgeSerializationStrategy.Instance.Serialize(new byte[0])
			);
			message.Serialize(buffer);
			IPagenatedMessage pm = _messageDestructor.BreakdownMessage(buffer);
			byte[] messageBuffer = pm.Buffer.CompressBytes();
			sender.Send(receiver, messageBuffer, messageBuffer.Length);
		}

		public IMessageReceiptSignature SendReliableMessage(IMessage message, ISocket sender, EndPoint receiver)
		{
			message.Receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			buffer.Append(
				ForgeSerializationStrategy.Instance.Serialize(GetMessageCode(message)),
				ForgeSerializationStrategy.Instance.Serialize(message.Receipt)
			);
			message.Serialize(buffer);
			IPagenatedMessage pm = _messageDestructor.BreakdownMessage(buffer);
			byte[] messageBuffer = pm.Buffer.CompressBytes();
			sender.Send(receiver, messageBuffer, messageBuffer.Length);

			if (_messageRepeater == null)
			{
				_messageRepeater = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepeater>();
				_messageRepeater.Start(_networkMediator);
			}

			_messageRepeater.AddMessageToRepeat(message, receiver);
			return message.Receipt;
		}

		public void ReceiveMessageBuffer(ISocket readingSocket, EndPoint messageSender, byte[] messageBuffer)
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
				bool isServer = _networkMediator.SocketFacade is ISocketServerFacade;

				// TODO:  There might not be an interpreter
				var interpreter = m.Interpreter;
				if (interpreter.ValidOnClient && !isServer)
					interpreter.Interpret(_networkMediator, messageSender, m);
				else if (interpreter.ValidOnServer && isServer)
					interpreter.Interpret(_networkMediator, messageSender, m);
			}
		}

		private static IMessage CreateMessageTypeFromBuffer(BMSByte buffer)
		{
			int code = buffer.GetBasicType<int>();
			return (IMessage)ForgeMessageCodes.Instantiate(code);
		}

		private void ProcessMessageSignature(ISocket readingSocket, EndPoint messageSender, BMSByte buffer, IMessage m)
		{
			var sig = ForgeSerializationStrategy.Instance.Deserialize<IMessageReceiptSignature>(buffer);
			if (sig != null)
			{
				m.Receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();
				m.Receipt = sig;
				SendMessage(new ForgeReceiptAcknowledgementMessage { ReceiptSignature = sig }, readingSocket, messageSender);
			}
		}
	}
}

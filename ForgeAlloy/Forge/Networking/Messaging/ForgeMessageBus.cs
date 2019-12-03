using System.Net;
using Forge.Factory;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageBus : IMessageBus
	{
		private const int TIME_TO_LIVE_STORED_MESSAGES = 10000;

		private readonly IMessageRepeater _messageRepeater;
		private readonly IMessageRepository _storedMessages;
		public IMessageBufferInterpreter MessageBufferInterpreter { get; private set; }
		private readonly IMessageDestructor _messageDestructor;
		private INetworkMediator _networkMediator;

		public ForgeMessageBus()
		{
			MessageBufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			_messageDestructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			_messageRepeater = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepeater>();
			_storedMessages = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
		}

		public void SetMediator(INetworkMediator networkMediator)
		{
			// TODO:  If a mediator is already set, then throw an exception
			_networkMediator = networkMediator;
			_networkMediator.PlayerRepository.onPlayerRemovedSubscription += PlayerRemovedFromRepository;
			_messageRepeater.Start(_networkMediator);
		}

		private void PlayerRemovedFromRepository(INetPlayer player)
		{
			MessageBufferInterpreter.ClearBufferFor(player);
		}

		private static int GetMessageCode(IMessage message)
		{
			return ForgeMessageCodes.GetCodeFromType(message.GetType());
		}

		public void SendMessage(IMessage message, ISocket sender, EndPoint receiver)
		{
			var buffer = new BMSByte();
			buffer.SetArraySize(128);
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(GetMessageCode(message)));
			if (message.Receipt != null)
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(message.Receipt));
			else
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(new byte[0]));
			message.Serialize(buffer);
			IPagenatedMessage pm = _messageDestructor.BreakdownMessage(buffer);
			byte[] messageBuffer = pm.Buffer.CompressBytes();
			sender.Send(receiver, messageBuffer, messageBuffer.Length);
		}

		public IMessageReceiptSignature SendReliableMessage(IMessage message, ISocket sender, EndPoint receiver)
		{
			message.Receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();
			SendMessage(message, sender, receiver);
			_messageRepeater.AddMessageToRepeat(message, receiver);
			return message.Receipt;
		}

		public void ReceiveMessageBuffer(ISocket readingSocket, EndPoint messageSender, byte[] messageBuffer)
		{
			var buffer = new BMSByte();
			buffer.Clone(messageBuffer);
			IMessageConstructor constructor = MessageBufferInterpreter.ReconstructPacketPage(buffer, messageSender);
			if (constructor.MessageReconstructed)
			{
				try
				{
					var m = (IMessage)ForgeMessageCodes.Instantiate(constructor.MessageBuffer.GetBasicType<int>());
					ProcessMessageSignature(readingSocket, messageSender, constructor.MessageBuffer, m);

					if (m.Receipt != null)
					{
						if (_storedMessages.Exists(m.Receipt))
							return;

						_storedMessages.AddMessage(m, messageSender, TIME_TO_LIVE_STORED_MESSAGES);
					}

					m.Deserialize(constructor.MessageBuffer);
					var interpreter = m.Interpreter;
					if (interpreter != null)
					{
						// TODO:  I don't like this type check and if branching in here...
						bool isServer = _networkMediator.SocketFacade is ISocketServerFacade;
						if (interpreter.ValidOnClient && !isServer)
							interpreter.Interpret(_networkMediator, messageSender, m);
						else if (interpreter.ValidOnServer && isServer)
							interpreter.Interpret(_networkMediator, messageSender, m);
					}
				}
				catch (MessageCodeNotFoundException) { }
			}
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

		public void MessageConfirmed(IMessageReceiptSignature messageReceipt)
		{
			_messageRepeater.RemoveRepeatingMessage(messageReceipt);
		}
	}
}

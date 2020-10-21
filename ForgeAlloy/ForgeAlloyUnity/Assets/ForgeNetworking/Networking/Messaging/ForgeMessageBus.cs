using System.Net;
using System.Threading;
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
		private readonly IMessageRepeater _messageRepeater;
		private readonly IMessageRepository _storedMessages;
		public IMessageBufferInterpreter MessageBufferInterpreter { get; private set; }
		private readonly IMessageDestructor _messageDestructor;
		private INetworkMediator _networkMediator;
		private BMSBytePool _bufferPool = new BMSBytePool();
		private MessagePool<ForgeReceiptAcknowledgementMessage> _msgAckPool = new MessagePool<ForgeReceiptAcknowledgementMessage>();
		private SynchronizationContext _synchronizationContext;

		public ForgeMessageBus()
		{
			MessageBufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			_messageDestructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			_messageRepeater = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepeater>();
			_storedMessages = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			_messageDestructor.BufferPool = _bufferPool;
			_synchronizationContext = SynchronizationContext.Current;
		}

		public void SetMediator(INetworkMediator networkMediator)
		{
			if (_networkMediator != null)
			{
				throw new MessageBussNetworkMediatorAlreadyAssignedException();
			}
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
			// TODO:  Possibly use the message interface to get the size needed for this
			BMSByte buffer = _bufferPool.Get(128);
			ForgeSerializer.Instance.Serialize(GetMessageCode(message), buffer);
			if (message.Receipt != null)
				ForgeSerializer.Instance.Serialize(message.Receipt, buffer);
			else
				ForgeSerializer.Instance.Serialize(false, buffer);
			message.Serialize(buffer);
			IPaginatedMessage pm = _messageDestructor.BreakdownMessage(buffer);
			sender.Send(receiver, pm.Buffer);
			message.Sent();
			_bufferPool.Release(buffer);
		}

		public IMessageReceiptSignature SendReliableMessage(IMessage message, ISocket sender, EndPoint receiver)
		{
			message.Receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();
			SendMessage(message, sender, receiver);
			_messageRepeater.AddMessageToRepeat(message, receiver);
			return message.Receipt;
		}

		// Note:  This is called from the read thread without sync context
		public void ReceiveMessageBuffer(ISocket readingSocket, EndPoint messageSender, BMSByte buffer)
		{
			IMessageConstructor constructor = MessageBufferInterpreter.ReconstructPacketPage(buffer, messageSender);
			if (constructor.MessageReconstructed)
			{
				try
				{
					var m = (IMessage)ForgeMessageCodes.Instantiate(constructor.MessageBuffer.GetBasicType<int>());
					ProcessMessageSignature(readingSocket, messageSender, constructor.MessageBuffer, m);

					if (m.Receipt != null)
					{
						if (_storedMessages.Exists(messageSender, m.Receipt))
							return;
						_storedMessages.AddMessage(m, messageSender, _networkMediator.PlayerTimeout);
					}

					m.Deserialize(constructor.MessageBuffer);
					var interpreter = m.Interpreter;
					if (interpreter != null)
					{
						_synchronizationContext.Post(InterpretWithinContext, new SocketContainerSynchronizationReadData
						{
							Interpreter = interpreter,
							Sender = messageSender,
							Message = m
						});
					}
				}
				catch (MessageCodeNotFoundException ex)
				{
					_networkMediator.EngineProxy.Logger.LogException(ex);
				}
				MessageBufferInterpreter.Release(constructor);
			}
		}

		private void InterpretWithinContext(object state)
		{
			var s = (SocketContainerSynchronizationReadData)state;

			if (ShouldInterpret(s))
				s.Interpreter.Interpret(_networkMediator, s.Sender, s.Message);
		}

		private bool ShouldInterpret(SocketContainerSynchronizationReadData readData)
		{
			return (_networkMediator.IsClient && readData.Interpreter.ValidOnClient) || (_networkMediator.IsServer && readData.Interpreter.ValidOnServer);
		}

		private void ProcessMessageSignature(ISocket readingSocket, EndPoint messageSender, BMSByte buffer, IMessage m)
		{
			var sig = ForgeSerializer.Instance.Deserialize<IMessageReceiptSignature>(buffer);
			if (sig != null)
			{
				m.Receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();
				m.Receipt = sig;
				ForgeReceiptAcknowledgementMessage ack = _msgAckPool.Get();
				ack.ReceiptSignature = sig;
				SendMessage(ack, readingSocket, messageSender);
			}
		}

		public void MessageConfirmed(EndPoint sender, IMessageReceiptSignature messageReceipt)
		{
			_messageRepeater.RemoveRepeatingMessage(sender, messageReceipt);
		}
	}
}

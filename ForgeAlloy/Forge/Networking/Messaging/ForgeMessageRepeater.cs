using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Forge.Factory;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageRepeater : IMessageRepeater
	{
		public int RepeatMillisecondsInterval { get; set; } = 16;
		private INetworkMediator _networkMediator;

		private readonly IMessageRepository _messageRepository;
		private CancellationTokenSource _socketTokenSourceRef;

		public ForgeMessageRepeater()
		{
			_messageRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
		}

		public void Start(INetworkMediator networkMediator)
		{
			_networkMediator = networkMediator;
			_socketTokenSourceRef = _networkMediator.SocketFacade.CancellationSource;
			Task.Run(RepeatInBackground, _socketTokenSourceRef.Token);
		}

		public void AddMessageToRepeat(IMessage message, EndPoint receiver)
		{
			lock (_messageRepository)
			{
				if (!_messageRepository.Exists(message.Receipt))
				{
					_messageRepository.AddMessage(message, receiver);
				}
			}
		}

		public void RemoveRepeatingMessage(IMessageReceiptSignature messageReceipt)
		{
			lock (_messageRepository)
			{
				_messageRepository.RemoveMessage(messageReceipt);
			}
		}

		public void RemoveAllFor(EndPoint receiver)
		{
			lock (_messageRepository)
			{
				_messageRepository.RemoveAllFor(receiver);
			}
		}

		private void RepeatInBackground()
		{
			// This starts when a reliable message is first created, so because of that
			// we should wait by the interval when we first enter this method
			Thread.Sleep(RepeatMillisecondsInterval);
			try
			{
				while (true)
				{
					_socketTokenSourceRef.Token.ThrowIfCancellationRequested();
					lock (_messageRepository)
					{
						var messageIterator = _messageRepository.GetIterator();
						foreach (var kv in messageIterator)
						{
							_networkMediator.MessageBus.SendReliableMessage(kv.Value,
								_networkMediator.SocketFacade.ManagedSocket, kv.Key);
						}
					}
					Thread.Sleep(RepeatMillisecondsInterval);
				}
			}
			catch (OperationCanceledException) { }
		}
	}
}

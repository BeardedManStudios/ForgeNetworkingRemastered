using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageRepository : IMessageRepository
	{
		private CancellationTokenSource _ttlBackgroundToken;

		private struct StoredMessage
		{
			public DateTime ttl;
			public IMessage message;
			public EndPoint sender;
		}

		private readonly List<StoredMessage> _messagesWithTTL = new List<StoredMessage>();
		private readonly Dictionary<IMessageReceipt, KeyValuePair<EndPoint, IMessage>> _messages = new Dictionary<IMessageReceipt, KeyValuePair<EndPoint, IMessage>>();

		public void Clear()
		{
			_ttlBackgroundToken.Cancel();
			lock (_messagesWithTTL)
			{
				_messagesWithTTL.Clear();
			}
			lock (_messages)
			{
				_messages.Clear();
			}
		}

		private void TTLBackground()
		{
			while (!_ttlBackgroundToken.IsCancellationRequested)
			{
				var now = DateTime.UtcNow;
				lock (_messagesWithTTL)
				{
					for (int i = 0; i < _messagesWithTTL.Count; i++)
					{
						if (_messagesWithTTL[i].ttl <= now)
						{
							RemoveFromMessageLookup(_messagesWithTTL[i].message.Receipt);
							_messagesWithTTL.RemoveAt(i--);
						}
					}

					if (_messagesWithTTL.Count == 0)
						break;
				}
				Thread.Sleep(10);
			}
		}

		public void AddMessage(IMessage message, EndPoint sender)
		{
			if (message.Receipt == null)
				throw new MessageRepositoryMissingReceiptOnMessageException();
			if (Exists(message.Receipt))
				throw new MessageWithReceiptSignatureAlreadyExistsException();
			lock (_messages)
			{
				_messages.Add(message.Receipt, new KeyValuePair<EndPoint, IMessage>(sender, message));
			}
		}

		public void AddMessage(IMessage message, EndPoint sender, int ttlMilliseconds)
		{
			if (ttlMilliseconds <= 0)
				throw new InvalidMessageRepositoryTTLProvided(ttlMilliseconds);

			AddMessage(message, sender);
			var span = new TimeSpan(0, 0, 0, 0, ttlMilliseconds);
			var now = DateTime.UtcNow;
			lock (_messagesWithTTL)
			{
				_messagesWithTTL.Add(new StoredMessage
				{
					ttl = now + span,
					message = message,
					sender = sender
				});
				if (_messagesWithTTL.Count == 1)
				{
					_ttlBackgroundToken = new CancellationTokenSource();
					Task.Run(TTLBackground, _ttlBackgroundToken.Token);
				}
			}
		}

		public void RemoveAllFor(EndPoint sender)
		{
			lock (_messages)
			{
				throw new NotImplementedException();
			}
		}

		public void RemoveMessage(IMessage message)
		{
			RemoveMessage(message.Receipt);
		}

		public void RemoveMessage(IMessageReceipt guid)
		{
			RemoveFromMessageLookup(guid);
			RemoveFromTTLWithGuid(guid);
		}

		private void RemoveFromMessageLookup(IMessageReceipt receipt)
		{
			lock (_messages)
			{
				_messages.Remove(receipt);
			}
		}

		public bool Exists(IMessageReceipt receipt)
		{
			lock (_messages)
			{
				return _messages.ContainsKey(receipt);
			}
		}

		private void RemoveFromTTLWithGuid(IMessageReceipt receipt)
		{
			lock (_messagesWithTTL)
			{
				for (int i = 0; i < _messagesWithTTL.Count; i++)
				{
					if (_messagesWithTTL[i].message.Receipt == receipt)
					{
						_messagesWithTTL.RemoveAt(i);
						break;
					}
				}
			}
		}

		public KeyValuePair<EndPoint, IMessage> Get(IMessageReceipt receipt)
		{
			return _messages[receipt];
		}

		public Dictionary<IMessageReceipt, KeyValuePair<EndPoint, IMessage>>.ValueCollection GetIterator()
		{
			return _messages.Values;
		}
	}
}

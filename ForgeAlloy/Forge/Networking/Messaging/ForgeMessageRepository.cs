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
		private readonly Dictionary<Guid, KeyValuePair<EndPoint, IMessage>> _messages = new Dictionary<Guid, KeyValuePair<EndPoint, IMessage>>();

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
							RemoveFromMessageLookup(_messagesWithTTL[i].message.Receipt.Signature);
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
				throw new MessageRepositoryMissingGuidOnMessageException();
			if (Exists(message.Receipt.Signature))
				throw new MessageWithReceiptSignatureAlreadyExistsException();
			lock (_messages)
			{
				_messages.Add(message.Receipt.Signature, new KeyValuePair<EndPoint, IMessage>(sender, message));
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

		public void RemoveMessage(IMessage message)
		{
			RemoveMessage(message.Receipt.Signature);
		}

		public void RemoveMessage(Guid guid)
		{
			RemoveFromMessageLookup(guid);
			RemoveFromTTLWithGuid(guid);
		}

		private void RemoveFromMessageLookup(Guid guid)
		{
			lock (_messages)
			{
				_messages.Remove(guid);
			}
		}

		public bool Exists(Guid guid)
		{
			lock (_messages)
			{
				return _messages.ContainsKey(guid);
			}
		}

		private void RemoveFromTTLWithGuid(Guid guid)
		{
			lock (_messagesWithTTL)
			{
				for (int i = 0; i < _messagesWithTTL.Count; i++)
				{
					if (_messagesWithTTL[i].message.Receipt.Signature == guid)
					{
						_messagesWithTTL.RemoveAt(i);
						break;
					}
				}
			}
		}

		public KeyValuePair<EndPoint, IMessage> Get(Guid guid)
		{
			return _messages[guid];
		}

		public IEnumerator<KeyValuePair<EndPoint, IMessage>> GetIterator() => _messages.Values.GetEnumerator();
	}
}

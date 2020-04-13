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
		private readonly Dictionary<EndPoint, Dictionary<IMessageReceiptSignature, IMessage>> _messages = new Dictionary<EndPoint, Dictionary<IMessageReceiptSignature, IMessage>>();

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
			try
			{
				while (true)
				{
					_ttlBackgroundToken.Token.ThrowIfCancellationRequested();
					var now = DateTime.UtcNow;
					lock (_messagesWithTTL)
					{
						for (int i = 0; i < _messagesWithTTL.Count; i++)
						{
							if (_messagesWithTTL[i].ttl <= now)
							{
								RemoveFromMessageLookup(_messagesWithTTL[i].sender, _messagesWithTTL[i].message.Receipt);
								_messagesWithTTL.RemoveAt(i--);
							}
						}

						if (_messagesWithTTL.Count == 0)
							break;
					}
					Thread.Sleep(10);
				}
			}
			catch (OperationCanceledException) { }
		}

		public void AddMessage(IMessage message, EndPoint sender)
		{
			if (message.Receipt == null)
				throw new MessageRepositoryMissingReceiptOnMessageException();
			if (Exists(sender, message.Receipt))
				throw new MessageWithReceiptSignatureAlreadyExistsException();
			lock (_messages)
			{
				if (!_messages.TryGetValue(sender, out var kv))
				{
					kv = new Dictionary<IMessageReceiptSignature, IMessage>();
					_messages.Add(sender, kv);
				}
				kv.Add(message.Receipt, message);
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
				var removals = new List<IMessageReceiptSignature>();
				_messages.Remove(sender);
			}
		}

		public void RemoveMessage(EndPoint sender, IMessage message)
		{
			RemoveMessage(sender, message.Receipt);
		}

		public void RemoveMessage(EndPoint sender, IMessageReceiptSignature guid)
		{
			RemoveFromMessageLookup(sender, guid);
			RemoveFromTTLWithGuid(guid);
		}

		private void RemoveFromMessageLookup(EndPoint sender, IMessageReceiptSignature receipt)
		{
			lock (_messages)
			{
				if (_messages.TryGetValue(sender, out var kv))
					kv.Remove(receipt);
			}
		}

		public bool Exists(EndPoint sender, IMessageReceiptSignature receipt)
		{
			bool exists = false;
			lock (_messages)
			{
				if (_messages.TryGetValue(sender, out var kv))
					exists = kv.ContainsKey(receipt);
			}
			return exists;
		}

		private void RemoveFromTTLWithGuid(IMessageReceiptSignature receipt)
		{
			lock (_messagesWithTTL)
			{
				for (int i = 0; i < _messagesWithTTL.Count; i++)
				{
					if (_messagesWithTTL[i].message.Receipt.Equals(receipt))
					{
						_messagesWithTTL.RemoveAt(i);
						break;
					}
				}
			}
		}

		public void Iterate(MessageRepositoryIterator iterator)
		{
			// TODO:  Review this for better performance
			var copy = new List<KeyValuePair<EndPoint, IMessage>>();
			lock (_messages)
			{
				foreach (var kv in _messages)
				{
					foreach (var mkv in kv.Value)
						copy.Add(new KeyValuePair<EndPoint, IMessage>(kv.Key, mkv.Value));
				}
			}
			foreach (var kv in copy)
				iterator(kv.Key, kv.Value);
		}
	}
}

using System.Collections.Generic;
using System.Net;
using Forge.Factory;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageBufferInterpreter : IMessageBufferInterpreter
	{
		private Dictionary<EndPoint, List<IMessageConstructor>> _messageConstructors = new Dictionary<EndPoint, List<IMessageConstructor>>();
		private readonly BMSBytePool _bufferPool = new BMSBytePool();

		public IMessageConstructor ReconstructPacketPage(BMSByte buffer, EndPoint sender)
		{
			int messageId = buffer.GetBasicType<int>();
			IMessageConstructor constructor = null;
			if (_messageConstructors.TryGetValue(sender, out var constructors))
			{
				foreach (var c in constructors)
				{
					if (c.Id == messageId)
					{
						constructor = c;
						ContinueProcessingExistingConstructor(buffer, constructor, sender);
						break;
					}
				}
			}

			if (constructor == null)
				constructor = ProcessNewConstructor(buffer, messageId, sender);
			return constructor;
		}

		private IMessageConstructor ProcessNewConstructor(BMSByte buffer, int messageId, EndPoint sender)
		{
			var constructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageConstructor>();
			constructor.Setup(_bufferPool, messageId);
			constructor.ReconstructMessagePage(buffer, sender);
			if (!constructor.MessageReconstructed)
			{
				if (_messageConstructors.TryGetValue(sender, out var constructors))
					constructors.Add(constructor);
				else
					_messageConstructors.Add(sender, new List<IMessageConstructor>() { constructor });
			}
			return constructor;
		}

		private void ContinueProcessingExistingConstructor(BMSByte buffer, IMessageConstructor constructor, EndPoint sender)
		{
			constructor.ReconstructMessagePage(buffer, sender);
			if (constructor.MessageReconstructed)
				_messageConstructors[sender].Remove(constructor);
		}

		public void ClearBufferFor(INetPlayer player)
		{
			_messageConstructors.Remove(player.EndPoint);
		}

		public void Release(IMessageConstructor constructor)
		{
			_bufferPool.Release(constructor.MessageBuffer);
		}
	}
}

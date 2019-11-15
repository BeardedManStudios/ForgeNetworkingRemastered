using System;
using System.Collections.Generic;
using System.Net;
using Forge.Factory;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageBufferInterpreter : IMessageBufferInterpreter
	{
		private Dictionary<Guid, IMessageConstructor> _messageConstructors = new Dictionary<Guid, IMessageConstructor>();

		public IMessageConstructor ReconstructPacketPage(BMSByte buffer, EndPoint sender)
		{
			var guid = Guid.Parse(buffer.GetBasicType<string>());
			IMessageConstructor constructor;
			if (_messageConstructors.TryGetValue(guid, out constructor))
				ContinueProcessingExistingConstructor(buffer, guid, constructor, sender);
			else
				constructor = ProcessNewConstructor(buffer, guid, sender);
			return constructor;
		}

		private IMessageConstructor ProcessNewConstructor(BMSByte buffer, Guid guid, EndPoint sender)
		{
			var constructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageConstructor>();
			constructor.ReconstructMessagePage(buffer, sender);
			if (!constructor.MessageReconstructed)
				_messageConstructors.Add(guid, constructor);
			return constructor;
		}

		private void ContinueProcessingExistingConstructor(BMSByte buffer, Guid guid, IMessageConstructor constructor, EndPoint sender)
		{
			constructor.ReconstructMessagePage(buffer, sender);
			if (constructor.MessageReconstructed)
				_messageConstructors.Remove(guid);
		}

		public void ClearBufferFor(INetPlayer player)
		{
			var removeKeys = new List<Guid>();
			foreach (var kv in _messageConstructors)
			{
				if (kv.Value.Sender == player.EndPoint)
					removeKeys.Add(kv.Key);
			}
			foreach (var key in removeKeys)
				_messageConstructors.Remove(key);
		}
	}
}

using System;
using System.Collections.Generic;
using Forge.Factory;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageBufferInterpreter : IMessageBufferInterpreter
	{
		private Dictionary<Guid, IMessageConstructor> _messageConstructors = new Dictionary<Guid, IMessageConstructor>();

		public IMessageConstructor ReconstructPacketPage(BMSByte buffer)
		{
			var guid = Guid.Parse(buffer.GetBasicType<string>());
			IMessageConstructor constructor;
			if (_messageConstructors.TryGetValue(guid, out constructor))
				ContinueProcessingExistingConstructor(buffer, guid, constructor);
			else
				constructor = ProcessNewConstructor(buffer, guid);
			return constructor;
		}

		private IMessageConstructor ProcessNewConstructor(BMSByte buffer, Guid guid)
		{
			var constructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageConstructor>();
			constructor.ReconstructMessagePage(buffer);
			if (!constructor.MessageReconstructed)
				_messageConstructors.Add(guid, constructor);
			return constructor;
		}

		private void ContinueProcessingExistingConstructor(BMSByte buffer, Guid guid, IMessageConstructor constructor)
		{
			constructor.ReconstructMessagePage(buffer);
			if (constructor.MessageReconstructed)
				_messageConstructors.Remove(guid);
		}
	}
}

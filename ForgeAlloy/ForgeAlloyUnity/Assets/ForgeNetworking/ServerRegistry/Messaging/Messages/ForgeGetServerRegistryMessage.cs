using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Serialization;
using Forge.ServerRegistry.Messaging.Interpreters;

namespace Forge.ServerRegistry.Messaging.Messages
{
	[ServerListingMessageContract(1, typeof(ForgeGetServerRegistryMessage))]
	public class ForgeGetServerRegistryMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter =>
			AbstractFactory.Get<INetworkTypeFactory>().GetNew<IGetServerRegistryInterpreter>();

		public override void Deserialize(BMSByte buffer)
		{
			// Nothing to serialize
		}

		public override void Serialize(BMSByte buffer)
		{
			// Nothing to serialize
		}
	}
}

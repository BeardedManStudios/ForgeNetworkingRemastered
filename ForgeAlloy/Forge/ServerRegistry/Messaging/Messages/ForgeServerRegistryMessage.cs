using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Serialization;
using Forge.ServerRegistry.DataStructures;
using Forge.ServerRegistry.Messaging.Interpreters;

namespace Forge.ServerRegistry.Messaging.Messages
{
	[ServerListingMessageContract(3, typeof(ForgeServerRegistryMessage))]
	public class ForgeServerRegistryMessage : ForgeMessage
	{
		public ServerListingEntry[] Entries { get; set; }
		public override IMessageInterpreter Interpreter =>
			AbstractFactory.Get<INetworkTypeFactory>().GetNew<IServerRegistryInterpreter>();

		public override void Deserialize(BMSByte buffer)
		{
			Entries = ForgeSerializer.Instance.Deserialize<ServerListingEntry[]>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(Entries, buffer);
		}
	}
}

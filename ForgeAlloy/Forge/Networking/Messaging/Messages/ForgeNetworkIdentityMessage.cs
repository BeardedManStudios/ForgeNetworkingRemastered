using Forge.Networking.Messaging.Interpreters;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	[MessageContract(4, typeof(ForgeNetworkIdentityMessage))]
	public class ForgeNetworkIdentityMessage : ForgeMessage
	{
		public IPlayerSignature Identity { get; set; }

		public override IMessageInterpreter Interpreter => new ForgeNetworkIdentityInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			Identity = ForgeSerializationStrategy.Instance.Deserialize<IPlayerSignature>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Identity));
		}
	}
}

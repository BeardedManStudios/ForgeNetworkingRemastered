using Forge.Networking.Messaging.Interpreters;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	[MessageContract(4, typeof(ForgeNetworkIdentityMessage))]
	public class ForgeNetworkIdentityMessage : ForgeMessage
	{
		public IPlayerSignature Identity { get; set; }

		public override IMessageInterpreter Interpreter => ForgeNetworkIdentityInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			Identity = ForgeSerializer.Instance.Deserialize<IPlayerSignature>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(Identity, buffer);
		}
	}
}

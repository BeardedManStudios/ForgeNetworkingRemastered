using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Serialization;
using Forge.ServerRegistry.Messaging.Interpreters;

namespace Forge.ServerRegistry.Messaging.Messages
{
	[ServerListingMessageContract(2, typeof(ForgeRegisterAsServerMessage))]
	public class ForgeRegisterAsServerMessage : ForgeMessage
	{
		public ushort Port { get; set; }
		public override IMessageInterpreter Interpreter =>
			AbstractFactory.Get<INetworkTypeFactory>().GetNew<IRegisterAsServerInterpreter>();

		public override void Deserialize(BMSByte buffer)
		{
			Port = ForgeSerializer.Instance.Deserialize<ushort>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(Port, buffer);
		}
	}
}

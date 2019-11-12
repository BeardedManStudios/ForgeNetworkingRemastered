using System;
using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeNetworkIdentityMessage : ForgeMessage
	{
		public Guid Identity { get; set; }

		public override IMessageInterpreter Interpreter => new ForgeNetworkIdentityInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			Identity = Guid.Parse(buffer.GetBasicType<string>());
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationContainer.Instance.Serialize(Identity.ToString()));
		}
	}
}

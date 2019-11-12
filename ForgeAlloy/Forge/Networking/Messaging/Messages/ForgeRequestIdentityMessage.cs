using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeRequestIdentityMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => new ForgeRequestIdentityInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			// Nothing to deserialize
		}

		public override void Serialize(BMSByte buffer)
		{
			// Nothing to serialize
		}
	}
}

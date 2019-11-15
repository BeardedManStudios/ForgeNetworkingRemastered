using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	[MessageContract(6, typeof(ForgeReadyForEngineMessage))]
	public class ForgeReadyForEngineMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => new ForgeReadyForEngineInterpreter();

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

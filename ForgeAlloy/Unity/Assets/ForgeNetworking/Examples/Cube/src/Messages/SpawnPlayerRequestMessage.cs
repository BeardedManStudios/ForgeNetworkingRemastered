using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(4, typeof(SpawnPlayerRequestMessage))]
	public class SpawnPlayerRequestMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => new SpawnPlayerRequestInterpreter();

		public override void Deserialize(BMSByte buffer)
		{

		}

		public override void Serialize(BMSByte buffer)
		{

		}
	}
}

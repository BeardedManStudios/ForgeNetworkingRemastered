using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(11, typeof(CreateMyPlayerMessage))]
	public class CreateMyPlayerMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => CreateMyPlayerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			
		}

		public override void Serialize(BMSByte buffer)
		{
			
		}
	}
}

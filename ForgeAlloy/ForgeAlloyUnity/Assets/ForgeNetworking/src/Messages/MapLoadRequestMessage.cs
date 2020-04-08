using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(1, typeof(MapLoadRequestMessage))]
	public class MapLoadRequestMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => MapLoadRequestInterpreter.Instance;
		public override void Deserialize(BMSByte buffer) { }
		public override void Serialize(BMSByte buffer) { }
	}
}

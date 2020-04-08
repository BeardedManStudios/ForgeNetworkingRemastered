using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(9, typeof(GetAllEntitiesRequestMessage))]
	public class GetAllEntitiesRequestMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => GetAllEntitiesRequestInterpreter.Instance;
		public override void Deserialize(BMSByte buffer) { }
		public override void Serialize(BMSByte buffer) { }
	}
}

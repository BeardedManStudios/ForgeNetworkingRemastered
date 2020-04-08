using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(2, typeof(MapLoadResponseMessage))]
	public class MapLoadResponseMessage : ForgeMessage
	{
		public string MapId { get; set; }
		public override IMessageInterpreter Interpreter => MapLoadResponseInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			MapId = buffer.GetBasicType<string>();
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(MapId, buffer);
		}
	}
}

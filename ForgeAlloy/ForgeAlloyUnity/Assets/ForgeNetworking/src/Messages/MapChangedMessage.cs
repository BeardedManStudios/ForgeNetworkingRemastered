using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(14, typeof(MapChangedMessage))]
	public class MapChangedMessage : ForgeMessage
	{
		public string MapName { get; set; }

		public override IMessageInterpreter Interpreter => MapChangedInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			MapName = ForgeSerializer.Instance.Deserialize<string>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(MapName, buffer);
		}
	}
}

using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeEntityMessage : ForgeMessage
	{
		public int EntityId { get; set; }

		public override IMessageInterpreter Interpreter => new ForgeEntityMessageInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			ObjectMapper.Instance.MapBytes(buffer, EntityId);
		}

		public override void Serialize(BMSByte buffer)
		{
			EntityId = buffer.GetBasicType<int>();
		}
	}
}

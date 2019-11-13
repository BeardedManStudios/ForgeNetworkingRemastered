using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeReceiptAcknowledgement : ForgeMessage
	{
		public string ReceiptGuid { get; set; }

		public override IMessageInterpreter Interpreter => new ForgeReceiptAcknolegementInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(ReceiptGuid));
		}

		public override void Serialize(BMSByte buffer)
		{
			ReceiptGuid = buffer.GetBasicType<string>();
		}
	}
}

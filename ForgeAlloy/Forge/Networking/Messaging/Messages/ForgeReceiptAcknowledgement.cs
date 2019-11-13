using Forge.DataStructures;
using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeReceiptAcknowledgement : ForgeMessage
	{
		public ISignature ReceiptGuid { get; set; }
		public override IMessageInterpreter Interpreter => new ForgeReceiptAcknolegementInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			ReceiptGuid = ForgeSerializationStrategy.Instance.Deserialize<ISignature>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(ReceiptGuid));
		}
	}
}

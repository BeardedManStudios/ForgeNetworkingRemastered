using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	[MessageContract(6, typeof(ForgeReceiptAcknowledgementMessage))]
	public class ForgeReceiptAcknowledgementMessage : ForgeMessage
	{
		public IMessageReceiptSignature ReceiptSignature { get; set; }
		public override IMessageInterpreter Interpreter => new ForgeReceiptAcknolegementInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			ReceiptSignature = ForgeSerializationStrategy.Instance.Deserialize<IMessageReceiptSignature>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(ReceiptSignature));
		}
	}
}

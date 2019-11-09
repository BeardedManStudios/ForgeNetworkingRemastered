using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public class ForgeReceiptAcknowledgement : ForgeMessage
	{
		public string ReceiptGuid { get; set; }

		public override IMessageInterpreter Interpreter => throw new System.NotImplementedException();

		public override void Deserialize(BMSByte buffer)
		{
			ObjectMapper.Instance.MapBytes(buffer, ReceiptGuid);
		}

		public override void Serialize(BMSByte buffer)
		{
			ReceiptGuid = buffer.GetBasicType<string>();
		}
	}
}

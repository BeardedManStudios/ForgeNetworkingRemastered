using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public abstract class ForgeMessage : IMessage
	{
		private IMessageReceiptSignature _receipt;

		public IMessageReceiptSignature Receipt
		{
			get => _receipt;
			set { _receipt = (IMessageReceiptSignature)value; }
		}

		public abstract IMessageInterpreter Interpreter { get; }
		public abstract void Serialize(BMSByte buffer);
		public abstract void Deserialize(BMSByte buffer);
	}
}

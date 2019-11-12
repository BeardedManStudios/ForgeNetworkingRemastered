using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public abstract class ForgeMessage : IMessage
	{
		private ForgeMessageReceipt _receipt;

		public IMessageReceipt Receipt
		{
			get => _receipt;
			set { _receipt = (ForgeMessageReceipt)value; }
		}

		public abstract IMessageInterpreter Interpreter { get; }
		public abstract void Serialize(BMSByte buffer);
		public abstract void Deserialize(BMSByte buffer);
	}
}

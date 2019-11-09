using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public abstract class ForgeMessage : IMessage
	{
		public int MessageCode { get; set; }

		private ForgeMessageReceipt _receipt;

		public IMessageReceipt Receipt
		{
			get => _receipt;
			set { _receipt = (ForgeMessageReceipt)value; }
		}

		public abstract IMessageInterpreter Interpreter { get; }
		public abstract void Serialize(BMSByte buffer);
		public abstract void Deserialize(BMSByte buffer);

		public void Interpret(INetworkContainer host)
		{
			this.GetHashCode();
			Interpreter.Interpret(host, this);
		}
	}
}

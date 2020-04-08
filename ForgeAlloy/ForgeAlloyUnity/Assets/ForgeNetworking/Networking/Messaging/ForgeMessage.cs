using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public abstract class ForgeMessage : IMessage
	{
		public event MessageSent OnMessageSent;
		public IMessageReceiptSignature Receipt { get; set; }
		public abstract IMessageInterpreter Interpreter { get; }
		public abstract void Serialize(BMSByte buffer);
		public abstract void Deserialize(BMSByte buffer);
		public void Sent()
		{
			OnMessageSent?.Invoke(this);
		}
	}
}

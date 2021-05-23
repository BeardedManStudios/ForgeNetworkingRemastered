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
		public bool IsPooled { get; set; } = false;
		public bool IsBuffered { get; set; } = false;
		public bool IsSent { get; set; } = false;
		public void Sent()
		{
			IsSent = true;
			OnMessageSent?.Invoke(this);
		}
		public void Unbuffered()
		{
			IsBuffered = false;
			OnMessageSent?.Invoke(this);
		}
	}
}

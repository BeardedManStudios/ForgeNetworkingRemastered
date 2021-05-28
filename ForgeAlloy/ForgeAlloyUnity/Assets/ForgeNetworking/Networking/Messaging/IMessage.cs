using Forge.Serialization;

namespace Forge.Networking.Messaging
{
	public delegate void MessageSent(IMessage message);

	public interface IMessage
	{
		event MessageSent OnMessageSent;
		IMessageReceiptSignature Receipt { get; set; }
		IMessageInterpreter Interpreter { get; }
		void Serialize(BMSByte buffer);
		void Deserialize(BMSByte buffer);
		void Sent();
		void Unbuffered();
		bool IsPooled { get; set; }
		bool IsBuffered { get; set; }
		bool IsSent { get; set; }

	}
}

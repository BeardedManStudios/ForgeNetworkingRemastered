using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(6, typeof(ChatMessage))]
	public class ChatMessage : ForgeMessage
	{
		public string Name { get; set; }
		public string Text { get; set; }

		public override IMessageInterpreter Interpreter => new ChatInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			Name = ForgeSerializationStrategy.Instance.Deserialize<string>(buffer);
			Text = ForgeSerializationStrategy.Instance.Deserialize<string>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Name));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Text));
		}
	}
}

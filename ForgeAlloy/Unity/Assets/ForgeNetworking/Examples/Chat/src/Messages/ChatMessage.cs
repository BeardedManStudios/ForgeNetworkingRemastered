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

		public override IMessageInterpreter Interpreter => ChatInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			Name = ForgeSerializer.Instance.Deserialize<string>(buffer);
			Text = ForgeSerializer.Instance.Deserialize<string>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(Name, buffer);
			ForgeSerializer.Instance.Serialize(Text, buffer);
		}
	}
}

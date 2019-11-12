using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeConnectChallengeResponseMessage : ForgeMessage, IChallengeResponseMessage
	{
		public byte[] ChallengeAttempt { get; set; }

		public override IMessageInterpreter Interpreter => new ForgeConnectChallengeResponseInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			ChallengeAttempt = ForgeSerializationContainer.Instance.Deserialize<byte[]>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationContainer.Instance.Serialize(ChallengeAttempt));
		}
	}
}

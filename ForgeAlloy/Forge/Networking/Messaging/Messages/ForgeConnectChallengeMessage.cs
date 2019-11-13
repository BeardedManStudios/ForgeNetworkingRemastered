using System;
using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	public class ForgeConnectChallengeMessage : ForgeMessage, IChallengeMessage
	{
		public byte[] Challenge { get; private set; }

		public ForgeConnectChallengeMessage()
		{
			Challenge = Guid.NewGuid().ToByteArray();
		}

		public override IMessageInterpreter Interpreter => new ForgeConnectChallengeInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			Challenge = ForgeSerializationStrategy.Instance.Deserialize<byte[]>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Challenge));
		}
	}
}

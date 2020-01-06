using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
	[MessageContract(2, typeof(ForgeConnectChallengeResponseMessage))]
	public class ForgeConnectChallengeResponseMessage : ForgeMessage, IChallengeResponseMessage
	{
		public byte[] ChallengeAttempt { get; set; }

		public override IMessageInterpreter Interpreter => new ForgeConnectChallengeResponseInterpreter();

		public override void Deserialize(BMSByte buffer)
		{
			ChallengeAttempt = ForgeSerializer.Instance.Deserialize<byte[]>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(ChallengeAttempt, buffer);
		}

		public void GenerateResponse(IChallengeMessage challenge)
		{
			ChallengeAttempt = new byte[challenge.Challenge.Length * 2];
			for (int i = 0; i < challenge.Challenge.Length; i++)
			{
				ChallengeAttempt[i] = challenge.Challenge[i];
				ChallengeAttempt[ChallengeAttempt.Length - i - 1] = challenge.Challenge[i];
			}
		}

		public bool ValidateResponse()
		{
			if (ChallengeAttempt.Length <= 1)
				return false;
			bool success = true;
			int len = ChallengeAttempt.Length / 2;
			for (int i = 0; i < len; i++)
			{
				if (ChallengeAttempt[i] != ChallengeAttempt[ChallengeAttempt.Length - i - 1])
				{
					success = false;
					break;
				}
			}
			return success;
		}
	}
}

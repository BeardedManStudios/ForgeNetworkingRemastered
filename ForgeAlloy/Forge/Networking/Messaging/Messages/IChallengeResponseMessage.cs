namespace Forge.Networking.Messaging.Messages
{
	public interface IChallengeResponseMessage : IMessage
	{
		byte[] ChallengeAttempt { get; set; }
	}
}

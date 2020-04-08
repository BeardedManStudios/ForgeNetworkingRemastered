namespace Forge.Networking.Messaging.Messages
{
	public interface IChallengeMessage : IMessage
	{
		byte[] Challenge { get; }
	}
}

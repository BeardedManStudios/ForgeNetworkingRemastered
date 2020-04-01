namespace Forge.Networking.Messaging.Messages
{
	public interface IChallengeResponseMessage : IMessage
	{
		byte[] ChallengeAttempt { get; set; }
		void GenerateResponse(IChallengeMessage challenge);
		bool ValidateResponse();
	}
}

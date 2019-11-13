using System.Net;
using Forge.Factory;
using Forge.Networking.Messaging.Messages;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeConnectChallengeInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkContainer netHost, EndPoint sender, IMessage message)
		{
			var challenge = (IChallengeMessage)message;
			var response = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IChallengeResponseMessage>();
			response.ChallengeAttempt = new byte[challenge.Challenge.Length * 2];
			for (int i = 0; i < challenge.Challenge.Length; i++)
			{
				response.ChallengeAttempt[i] = challenge.Challenge[i];
				response.ChallengeAttempt[response.ChallengeAttempt.Length - i - 1] = challenge.Challenge[i];
			}
			netHost.MessageBus.SendReliableMessage(response, netHost.SocketContainer.ManagedSocket, sender);
		}
	}
}

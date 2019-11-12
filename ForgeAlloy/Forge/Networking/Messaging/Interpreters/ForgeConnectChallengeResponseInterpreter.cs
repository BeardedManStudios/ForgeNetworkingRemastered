using System.Net;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeConnectChallengeResponseInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkContainer netContainer, EndPoint sender, IMessage message)
		{
			var response = (IChallengeResponseMessage)message;
			if (response.ChallengeAttempt.Length <= 1)
				return;
			bool success = true;
			int len = response.ChallengeAttempt.Length / 2;
			for (int i = 0; i < len; i++)
			{
				if (response.ChallengeAttempt[i] != response.ChallengeAttempt[response.ChallengeAttempt.Length - i - 1])
				{
					success = false;
					break;
				}
			}
			if (!success)
				return;
			try
			{
				var serverContainer = (ISocketServerContainer)netContainer.SocketContainer;
				serverContainer.ChallengeSuccess(netContainer, sender);
			}
			catch (PlayerNotFoundException) { }
		}
	}
}

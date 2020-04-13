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

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var response = (IChallengeResponseMessage)message;
			if (!response.ValidateResponse())
				return;
			try
			{
				var serverContainer = (ISocketServerFacade)netMediator.SocketFacade;
				serverContainer.ChallengeSuccess(netMediator, sender);
			}
			catch (PlayerNotFoundException ex)
			{
				netMediator.EngineProxy.Logger.LogException(ex);
			}
		}
	}
}

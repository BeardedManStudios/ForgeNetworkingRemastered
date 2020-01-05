using System.Net;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeNetworkIdentityInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var identityMessage = (ForgeNetworkIdentityMessage)message;
			var clientContainer = (ISocketClientFacade)netMediator.SocketFacade;
			clientContainer.NetPlayerId = identityMessage.Identity;
			var engineReadyMessage = new ForgeReadyForEngineMessage();
			netMediator.SocketFacade.Established(netMediator);
			netMediator.EngineProxy.NetworkingEstablished();
			netMediator.MessageBus.SendReliableMessage(engineReadyMessage, netMediator.SocketFacade.ManagedSocket, sender);
		}
	}
}

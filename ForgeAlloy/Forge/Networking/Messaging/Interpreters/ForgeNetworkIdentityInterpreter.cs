using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;
using System.Net;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeNetworkIdentityInterpreter : IMessageInterpreter
	{
		public static ForgeNetworkIdentityInterpreter Instance { get; private set; } = new ForgeNetworkIdentityInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var identityMessage = (ForgeNetworkIdentityMessage)message;
			var clientContainer = (ISocketClientFacade)netMediator.SocketFacade;
			clientContainer.NetPlayerId = identityMessage.Identity;
			var engineReadyMessage = new ForgeReadyForEngineMessage();
			((ISocketClientFacade)netMediator.SocketFacade).Established(netMediator);
			netMediator.EngineProxy.NetworkingEstablished();
			netMediator.MessageBus.SendReliableMessage(engineReadyMessage, netMediator.SocketFacade.ManagedSocket, sender);
		}
	}
}

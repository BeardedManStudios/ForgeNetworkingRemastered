using System.Net;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeNetworkIdentityInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkContainer netHost, EndPoint sender, IMessage message)
		{
			var identityMessage = (ForgeNetworkIdentityMessage)message;
			var clientContainer = (ISocketClientContainer)netHost.SocketContainer;
			clientContainer.NetPlayerId = identityMessage.Identity;
			var engineReadyMessage = new ForgeReadyForEngineMessage();
			netHost.MessageBus.SendReliableMessage(engineReadyMessage, netHost.SocketContainer.ManagedSocket, sender);
		}
	}
}

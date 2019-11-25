using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapLoadRequestInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;

		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			Debug.Log("Map Load Request Received");
			//var identityMessage = (MapLoadMessage)message;
			//var clientContainer = (ISocketClientFacade)netMediator.SocketFacade;
			//clientContainer.NetPlayerId = identityMessage.Identity;
			var mapLoadResponseMessage = new MapLoadResponseMessage() { MapId = "Cube" };
			//netMediator.EngineProxy.NetworkingEstablished();
			netMediator.MessageBus.SendReliableMessage(mapLoadResponseMessage, netMediator.SocketFacade.ManagedSocket, sender);
		}
	}
}

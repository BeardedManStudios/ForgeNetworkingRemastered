using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapLoadRequestInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			Debug.Log("Map Load Request Received");
			var mapLoadResponseMessage = new MapLoadResponseMessage() { MapId = "Cube" };
			netMediator.MessageBus.SendReliableMessage(mapLoadResponseMessage, netMediator.SocketFacade.ManagedSocket, sender);
		}
	}
}

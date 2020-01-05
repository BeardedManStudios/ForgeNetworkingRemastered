using System.Net;
using Forge.Networking.Messaging;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapLoadRequestInterpreter : IMessageInterpreter
	{
		public static MapLoadRequestInterpreter Instance { get; private set; } = new MapLoadRequestInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			Debug.Log("Map Load Request Received");
			var e = (IEngineFacade)netMediator.EngineProxy;
			var mapLoadResponseMessage = new MapLoadResponseMessage() { MapId = e.CurrentMap };
			netMediator.MessageBus.SendReliableMessage(mapLoadResponseMessage, netMediator.SocketFacade.ManagedSocket, sender);
		}
	}
}

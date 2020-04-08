using Forge.Networking.Messaging;
using System.Net;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapLoadRequestInterpreter : IMessageInterpreter
	{
		public static MapLoadRequestInterpreter Instance { get; private set; } = new MapLoadRequestInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var mapLoadResponseMessage = new MapLoadResponseMessage() { MapId = SceneManager.GetActiveScene().name };
			netMediator.MessageBus.SendReliableMessage(mapLoadResponseMessage, netMediator.SocketFacade.ManagedSocket, sender);
		}
	}
}

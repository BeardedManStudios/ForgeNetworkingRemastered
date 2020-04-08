using Forge.Networking.Messaging;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapLoadResponseInterpreter : IMessageInterpreter
	{
		public static MapLoadResponseInterpreter Instance { get; private set; } = new MapLoadResponseInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var responseMessage = (MapLoadResponseMessage)message;
			Debug.Log($"Map Load Response Received - { responseMessage.MapId }");
			SceneManager.LoadScene(responseMessage.MapId);
			// Now request the entities from the server
			//netMediator.SendReliableMessage(new GetAllEntitiesRequestMessage());
		}
	}
}

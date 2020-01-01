using System.Net;
using Forge.Networking.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapLoadResponseInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var responseMessage = (MapLoadResponseMessage)message;
			Debug.Log($"Map Load Response Received - { responseMessage.MapId }");
			SceneManager.LoadScene(responseMessage.MapId);
		}
	}
}

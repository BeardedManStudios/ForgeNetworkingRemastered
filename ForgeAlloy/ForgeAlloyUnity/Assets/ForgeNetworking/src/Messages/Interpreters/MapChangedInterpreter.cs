using Forge.Networking.Messaging;
using System.Net;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class MapChangedInterpreter : IMessageInterpreter
	{
		public static MapChangedInterpreter Instance { get; private set; } = new MapChangedInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var m = (MapChangedMessage)message;
			SceneManager.LoadScene(m.MapName);
		}
	}
}
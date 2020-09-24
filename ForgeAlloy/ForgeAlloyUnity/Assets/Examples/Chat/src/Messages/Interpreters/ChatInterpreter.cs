using System.Net;
using Forge.Networking.Messaging;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class ChatInterpreter : IMessageInterpreter
	{
		public static ChatInterpreter Instance { get; private set; } = new ChatInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator,
			EndPoint sender, IMessage message)
		{
			var m = (ChatMessage)message;
			var cl = GameObject.FindObjectOfType<ChatListener>();
			cl.PrintMessage(m.Name, m.Text);
			if (netMediator.IsServer)
				netMediator.ForwardToOtherClients(sender, message);
		}
	}
}

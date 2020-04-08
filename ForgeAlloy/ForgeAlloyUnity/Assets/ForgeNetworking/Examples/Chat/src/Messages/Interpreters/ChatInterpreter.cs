using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class ChatInterpreter : IMessageInterpreter
	{
		public static ChatInterpreter Instance { get; private set; } = new ChatInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var m = (ChatMessage)message;
			var cl = GameObject.FindObjectOfType<ChatListener>();
			cl.PrintMessage(m.Name, m.Text);
			if (IsServer(netMediator))
				SendMessageToOtherClients(netMediator, sender, message);
		}

		private bool IsServer(INetworkMediator netMediator)
		{
			return netMediator.SocketFacade is ISocketServerFacade;
		}

		private void SendMessageToOtherClients(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var itr = netMediator.PlayerRepository.GetEnumerator();
			while (itr.MoveNext())
			{
				if (itr.Current != null && itr.Current.EndPoint != sender)
					netMediator.SendReliableMessage(message, itr.Current.EndPoint);
			}
		}
	}
}

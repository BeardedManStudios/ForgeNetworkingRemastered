using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Puzzle.Network;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CreateMyPlayerInterpreter : IMessageInterpreter
	{
		public static CreateMyPlayerInterpreter Instance { get; private set; } = new CreateMyPlayerInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			INetPlayer player = netMediator.PlayerRepository.GetPlayer(sender);
			GameObject.FindObjectOfType<ComplexSampleNetwork>().PlayerJoined(player);
		}
	}
}

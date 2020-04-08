using System.Net;
using Forge.Networking.Messaging;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CreateMyPlayerInterpreter : IMessageInterpreter
	{
		public static CreateMyPlayerInterpreter Instance { get; private set; } = new CreateMyPlayerInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			//INetPlayer player = netMediator.PlayerRepository.GetPlayer(sender);
			//GameObject.FindObjectOfType<PuzzleNetwork>().PlayerJoined(player);
		}
	}
}

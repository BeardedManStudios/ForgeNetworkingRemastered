using System.Net;
using Forge.Networking.Players;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeReadyForEngineInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netHost, EndPoint sender, IMessage message)
		{
			try
			{
				INetPlayer player = netHost.PlayerRepository.GetPlayer(sender);
				if (!player.IsInEngine)
				{
					player.IsInEngine = true;
					netHost.EngineContainer.PlayerJoined(player);
				}
			}
			catch (PlayerNotFoundException) { }
		}
	}
}

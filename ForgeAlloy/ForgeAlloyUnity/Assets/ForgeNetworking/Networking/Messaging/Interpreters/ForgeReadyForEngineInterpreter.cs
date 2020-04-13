using System.Net;
using Forge.Networking.Players;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeReadyForEngineInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			try
			{
				INetPlayer player = netMediator.PlayerRepository.GetPlayer(sender);
				if (!player.IsInEngine)
					player.IsInEngine = true;
			}
			catch (PlayerNotFoundException ex)
			{
				netMediator.EngineProxy.Logger.LogException(ex);
			}
		}
	}
}

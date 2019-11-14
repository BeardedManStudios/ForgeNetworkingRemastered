using System.Net;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.ServerRegistry.Messaging.Interpreters;
using ForgeServerRegistryService.Networking.Players;

namespace ForgeServerRegistryService.Messaging.Interpreters
{
	public class RegisterAsServerInterpreter : IRegisterAsServerInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netContainer, EndPoint sender, IMessage message)
		{
			var player = (NetworkPlayer)netContainer.PlayerRepository.GetPlayer(sender);
			player.IsRegisteredServer = true;
		}
	}
}

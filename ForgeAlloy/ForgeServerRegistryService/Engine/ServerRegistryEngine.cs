using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;

namespace ForgeServerRegistryService.Engine
{
	public class ServerRegistryEngine : IEngineProxy
	{
		public IEntityRepository EntityRepository { get; set; }

		public ServerRegistryEngine()
		{
			EntityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
		}

		public void PlayerJoined(INetPlayer newPlayer)
		{
			// We will handle the player joining event on the player repository
			return;
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message, EndPoint sender)
		{
			// Not expecting to have any entities on the server registry engine
			return;
		}
	}
}

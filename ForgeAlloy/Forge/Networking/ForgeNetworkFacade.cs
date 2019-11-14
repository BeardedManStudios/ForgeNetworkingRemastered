using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public class ForgeNetworkFacade : INetworkMediator
	{
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineProxy EngineContainer { get; private set; }
		public IMessageBus MessageBus { get; private set; }
		public ISocketFacade SocketFacade { get; private set; }

		public ForgeNetworkFacade()
		{
			PlayerRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			MessageBus = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBus>();
		}

		public void ChangeEngineFacade(IEngineProxy engineContainer)
		{
			EngineContainer = engineContainer;
		}

		public void StartServer(ushort port, int maxPlayers)
		{
			var server = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			SocketFacade = server;
			server.StartServer(port, maxPlayers, this);
		}

		public void StartClient(string hostAddress, ushort port)
		{
			var client = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketClientFacade>();
			SocketFacade = client;
			client.StartClient(hostAddress, port, this);
		}

		public void SendMessage(IMessage message, IPlayerSignature playerId)
		{
			INetPlayer player = PlayerRepository.GetPlayer(playerId);
			SendMessage(message, player);
		}

		public void SendMessage(IMessage message, INetPlayer player)
		{
			MessageBus.SendMessage(message, SocketFacade.ManagedSocket, player.EndPoint);
		}
	}
}

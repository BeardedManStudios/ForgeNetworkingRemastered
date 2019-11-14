using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public class ForgeNetworkMediator : INetworkMediator
	{
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineProxy EngineProxy { get; private set; }
		public IMessageBus MessageBus { get; private set; }
		public ISocketFacade SocketFacade { get; private set; }
		private readonly IPlayerTimeoutBridge _timeoutBridge;

		public ForgeNetworkMediator()
		{
			PlayerRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			MessageBus = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBus>();
			_timeoutBridge = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerTimeoutBridge>();
		}

		public void ChangeEngineProxy(IEngineProxy engineProxy)
		{
			EngineProxy = engineProxy;
		}

		public void StartServer(ushort port, int maxPlayers)
		{
			var server = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			SocketFacade = server;
			server.StartServer(port, maxPlayers, this);
			_timeoutBridge.StartWatching(this);
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

using System.Threading;
using System.Threading.Tasks;
using Forge.Networking.Players;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketServerContainer : ForgeUDPSocketContainerBase, ISocketServerContainer
	{
		private const int MAX_PARALLEL_CONNECTION_REQUEST = 64;

		private readonly IServerSocket _socket;
		public override ISocket ManagedSocket => _socket;
		private CancellationTokenSource _newConnectionsTokenSource;

		public ForgeUDPSocketServerContainer()
		{
			_socket = ForgeTypeFactory.GetNew<IServerSocket>();
		}

		public void StartServer(string address, ushort port, int maxPlayers, INetworkContainer netContainer)
		{
			this.netContainer = netContainer;
			_socket.Listen(address, port, MAX_PARALLEL_CONNECTION_REQUEST);
			_newConnectionsTokenSource = new CancellationTokenSource();
			readTokenSource = new CancellationTokenSource();
			Task.Run(ListenForConnections, _newConnectionsTokenSource.Token);
			Task.Run(ReadNetwork, readTokenSource.Token);
		}

		public override void ShutDown()
		{
			_newConnectionsTokenSource.Cancel();
			base.ShutDown();
		}

		private void ListenForConnections()
		{
			while (true)
			{
				ISocket newClient = _socket.AwaitAccept();
				synchronizationContext.Post(SynchronizedPlayerConnected, newClient);
			}
		}

		private void SynchronizedPlayerConnected(object state)
		{
			var socket = (ISocket)state;
			var newPlayer = ForgeTypeFactory.GetNew<INetPlayer>();
			newPlayer.Socket = socket;
			netContainer.PlayerRepository.AddPlayer(newPlayer);
			netContainer.EngineContainer.PlayerJoined(newPlayer);
		}
	}
}

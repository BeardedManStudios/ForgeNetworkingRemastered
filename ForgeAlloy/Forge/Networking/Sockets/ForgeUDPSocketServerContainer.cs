using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketServerContainer : ISocketServerContainer
	{
		private const int MAX_PARALLEL_CONNECTION_REQUEST = 64;

		public ISocket ManagedSocket => _socket;
		private readonly IServerSocket _socket;
		private CancellationTokenSource _newConnectionsTokenSource;
		private CancellationTokenSource _readTokenSource;
		private INetworkContainer _netContainer;
		private SynchronizationContext _synchronizationContext;

		public ForgeUDPSocketServerContainer()
		{
			_socket = ForgeTypeFactory.GetNew<IServerSocket>();
			_synchronizationContext = SynchronizationContext.Current;
		}

		public void StartServer(string address, ushort port, int maxPlayers, INetworkContainer netContainer)
		{
			_netContainer = netContainer;
			_socket.Listen(address, port, MAX_PARALLEL_CONNECTION_REQUEST);
			_newConnectionsTokenSource = new CancellationTokenSource();
			_readTokenSource = new CancellationTokenSource();
			Task.Run(ListenForConnections, _newConnectionsTokenSource.Token);
			Task.Run(ReadNetwork, _readTokenSource.Token);
		}

		public void ShutDown()
		{
			_newConnectionsTokenSource.Cancel();
			_readTokenSource.Cancel();
		}

		private void ListenForConnections()
		{
			while (true)
			{
				ISocket newClient = _socket.AwaitAccept();
				_synchronizationContext.Post(SynchronizedPlayerConnected, newClient);
			}
		}

		private void SynchronizedPlayerConnected(object state)
		{
			var socket = (ISocket)state;
			var newPlayer = ForgeTypeFactory.GetNew<INetPlayer>();
			newPlayer.Socket = socket;
			_netContainer.PlayerRepository.AddPlayer(newPlayer);
			_netContainer.EngineContainer.PlayerJoined(newPlayer);
		}

		private void ReadNetwork()
		{
			EndPoint readEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
			var buffer = new BMSByte();
			buffer.SetArraySize(2048);
			while (true)
			{
				buffer.Clear();
				int readLen = _socket.Receive(buffer, ref readEp);
				_synchronizationContext.Post(SynchronizedMessageRead, new SocketContainerSynchronizationReadData
				{
					Buffer = buffer.CompressBytes(),
					Endpoint = readEp
				});
			}
		}

		private void SynchronizedMessageRead(object state)
		{
			var data = (SocketContainerSynchronizationReadData)state;
			INetPlayer player = _netContainer.PlayerRepository.GetPlayer(data.Endpoint);
			_netContainer.MessageBus.ReceiveMessageBuffer(_netContainer, ManagedSocket,
				player.Socket, data.Buffer);
		}
	}
}

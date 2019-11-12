using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketServerContainer : ForgeUDPSocketContainerBase, ISocketServerContainer
	{
		private const int MAX_PARALLEL_CONNECTION_REQUEST = 64;

		private readonly IServerSocket _socket;
		public override ISocket ManagedSocket => _socket;
		private CancellationTokenSource _newConnectionsTokenSource;

		private IPlayerRepository _challengedPlayers;

		public ForgeUDPSocketServerContainer()
		{
			_socket = ForgeTypeFactory.GetNew<IServerSocket>();
			_challengedPlayers = ForgeTypeFactory.GetNew<IPlayerRepository>();
		}

		public void StartServer(ushort port, int maxPlayers, INetworkContainer netContainer)
		{
			// TODO:  Use maxPlayers
			this.netContainer = netContainer;
			_socket.Listen(port, MAX_PARALLEL_CONNECTION_REQUEST);
			_newConnectionsTokenSource = new CancellationTokenSource();
			readTokenSource = new CancellationTokenSource();
			Task.Run(ReadNetwork, readTokenSource.Token);
		}

		public override void ShutDown()
		{
			_newConnectionsTokenSource.Cancel();
			base.ShutDown();
		}

		public void ChallengeSuccess(INetworkContainer netContainer, EndPoint endpoint)
		{
			INetPlayer player = _challengedPlayers.GetPlayer(endpoint);
			var netIdentity = new ForgeNetworkIdentityMessage
			{
				Identity = player.Id
			};
			_challengedPlayers.RemovePlayer(player);
			netContainer.PlayerRepository.AddPlayer(player);
			netContainer.MessageBus.SendReliableMessage(netIdentity, ManagedSocket, endpoint);
		}

		protected override void ProcessMessageRead(SocketContainerSynchronizationReadData data)
		{
			// TODO:  Check if the player has been banned and only do the following if not
			if (!netContainer.PlayerRepository.Exists(data.Endpoint))
			{
				var newPlayer = ForgeTypeFactory.GetNew<INetPlayer>();
				newPlayer.EndPoint = data.Endpoint;
				_challengedPlayers.AddPlayer(newPlayer);
			}
			else
				base.ProcessMessageRead(data);
		}
	}
}

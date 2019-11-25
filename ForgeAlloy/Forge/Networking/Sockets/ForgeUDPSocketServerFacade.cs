using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Forge.Factory;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketServerFacade : ForgeUDPSocketFacadeBase, ISocketServerFacade
	{
		private const int CHALLENGED_PLAYER_TTL = 5000;
		private const int MAX_PARALLEL_CONNECTION_REQUEST = 64;

		private readonly IServerSocket _socket;
		public override ISocket ManagedSocket => _socket;
		private readonly List<EndPoint> _bannedEndpoints = new List<EndPoint>();

		private IPlayerRepository _challengedPlayers;

		public ForgeUDPSocketServerFacade()
		{
			_socket = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IServerSocket>();
			_challengedPlayers = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
		}

		public void StartServer(ushort port, int maxPlayers, INetworkMediator netMediator)
		{
			networkMediator = netMediator;
			_socket.Listen(port, MAX_PARALLEL_CONNECTION_REQUEST);
			CancellationSource = new CancellationTokenSource();
			Task.Run(ReadNetwork, CancellationSource.Token);
		}

		public override void ShutDown()
		{
			CancellationSource.Cancel();
			base.ShutDown();
		}

		public void ChallengeSuccess(INetworkMediator netContainer, EndPoint endpoint)
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
			if (_bannedEndpoints.Contains(data.Endpoint))
				return;
			else if (!networkMediator.PlayerRepository.Exists(data.Endpoint))
			{
				CleanupOldChallengedPlayers();
				if (!_challengedPlayers.Exists(data.Endpoint))
				{
					var newPlayer = AbstractFactory.Get<INetworkTypeFactory>().GetNew<INetPlayer>();
					newPlayer.EndPoint = data.Endpoint;
					newPlayer.LastCommunication = DateTime.Now;
					_challengedPlayers.AddPlayer(newPlayer);
					var challengeMessage = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IChallengeMessage>();
					networkMediator.MessageBus.SendReliableMessage(challengeMessage, ManagedSocket, data.Endpoint);
				}
				else
					ProcessPlayerMessageRead(_challengedPlayers.GetPlayer(data.Endpoint), data.Buffer);
			}
			else
				base.ProcessMessageRead(data);
		}

		protected override void ProcessMessageRead(SocketContainerSynchronizationReadData data)
		{
			// TODO:  Should check if player is banned
			INetPlayer player = networkMediator.PlayerRepository.GetPlayer(data.Endpoint);
			ProcessPlayerMessageRead(player, data.Buffer);
		}

		protected void ProcessPlayerMessageRead(INetPlayer player, byte[] buffer)
		{
			player.LastCommunication = DateTime.Now;
			networkMediator.MessageBus.ReceiveMessageBuffer(ManagedSocket,
				player.EndPoint, buffer);
		}

		private void CleanupOldChallengedPlayers()
		{
			var now = DateTime.Now;
			var players = _challengedPlayers.GetEnumerator();
			List<INetPlayer> removals = new List<INetPlayer>();
			while (players.MoveNext())
			{
				var p = players.Current;
				TimeSpan len = now - p.LastCommunication;
				if (len.TotalMilliseconds >= CHALLENGED_PLAYER_TTL)
					removals.Add(p);
			}
			foreach (var p in removals)
				_challengedPlayers.RemovePlayer(p);
		}
	}
}

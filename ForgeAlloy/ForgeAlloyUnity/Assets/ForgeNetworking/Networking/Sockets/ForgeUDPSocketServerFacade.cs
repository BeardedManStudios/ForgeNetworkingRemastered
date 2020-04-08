using Forge.Factory;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;
using Forge.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketServerFacade : ForgeUDPSocketFacadeBase, ISocketServerFacade
	{
		private const int CHALLENGED_PLAYER_TTL = 5000;
		private const int MAX_PARALLEL_CONNECTION_REQUEST = 64;

		private readonly IServerSocket _socket;
		public override ISocket ManagedSocket => _socket;

		public IPlayerSignature NetPlayerId { get; set; }

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
			NetPlayerId = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
			Task.Run(ReadNetwork, CancellationSource.Token);
		}

		public override void ShutDown()
		{
			CancellationSource.Cancel();
			base.ShutDown();
		}

		public void ChallengeSuccess(INetworkMediator netContainer, EndPoint endpoint)
		{
			INetPlayer player;
			ForgeNetworkIdentityMessage netIdentity;
			lock (_challengedPlayers)
			{
				player = _challengedPlayers.GetPlayer(endpoint);
				netContainer.PlayerRepository.AddPlayer(player);
				netIdentity = new ForgeNetworkIdentityMessage
				{
					Identity = player.Id
				};
				_challengedPlayers.RemovePlayer(player);
			}
			netContainer.MessageBus.SendReliableMessage(netIdentity, ManagedSocket, endpoint);
		}

		protected override void ProcessMessageRead(BMSByte buffer, EndPoint sender)
		{
			if (_bannedEndpoints.Contains(sender))
				return;
			else if (!networkMediator.PlayerRepository.Exists(sender))
			{
				CleanupOldChallengedPlayers();
				if (!_challengedPlayers.Exists(sender))
					synchronizationContext.Post(CreatePlayer, sender);
				else
					ProcessPlayerMessageRead(_challengedPlayers.GetPlayer(sender), buffer);
			}
			else
			{
				INetPlayer player = networkMediator.PlayerRepository.GetPlayer(sender);
				ProcessPlayerMessageRead(player, buffer);
			}
		}

		private void CreatePlayer(object state)
		{
			var sender = (EndPoint)state;
			var newPlayer = AbstractFactory.Get<INetworkTypeFactory>().GetNew<INetPlayer>();
			newPlayer.EndPoint = sender;
			newPlayer.LastCommunication = DateTime.Now;
			_challengedPlayers.AddPlayer(newPlayer);
			var challengeMessage = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IChallengeMessage>();
			networkMediator.MessageBus.SendReliableMessage(challengeMessage, ManagedSocket, sender);
		}

		protected void ProcessPlayerMessageRead(INetPlayer player, BMSByte buffer)
		{
			player.LastCommunication = DateTime.Now;
			networkMediator.MessageBus.ReceiveMessageBuffer(ManagedSocket, player.EndPoint, buffer);
		}

		private void CleanupOldChallengedPlayers()
		{
			lock (_challengedPlayers)
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
}

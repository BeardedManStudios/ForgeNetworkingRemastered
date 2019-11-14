using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Networking.Players
{
	public class ForgePlayerTimeoutBridge : IPlayerTimeoutBridge
	{
		public int TimeoutMilliseconds { get; set; } = 10000;
		private readonly List<INetPlayer> _previousPlayerSet = new List<INetPlayer>();
		private readonly List<INetPlayer> _timedOutPlayers = new List<INetPlayer>();
		private INetworkMediator _networkMediator;
		private SynchronizationContext _sourceSyncCtx;

		public void StartWatching(INetworkMediator mediator)
		{
			_networkMediator = mediator;
			_sourceSyncCtx = SynchronizationContext.Current;
			Task.Run(WatchForTimeouts, _networkMediator.SocketFacade.CancellationSource.Token);
		}

		private void WatchForTimeouts()
		{
			while (!_networkMediator.SocketFacade.CancellationSource.IsCancellationRequested)
			{
				var now = DateTime.Now;
				lock (_previousPlayerSet)
				{
					foreach (var player in _previousPlayerSet)
					{
						var span = now - player.LastCommunication;
						if (span.TotalMilliseconds >= TimeoutMilliseconds)
							_timedOutPlayers.Add(player);
					}
				}
				_sourceSyncCtx.Post(ProcessPlayerLists, null);
				Thread.Sleep(TimeoutMilliseconds);
			}
		}

		private void ProcessPlayerLists(object state)
		{
			lock (_previousPlayerSet)
			{
				_previousPlayerSet.Clear();
				var itr = _networkMediator.PlayerRepository.GetEnumerator();
				while (itr.MoveNext())
				{
					if (itr.Current != null)
						_previousPlayerSet.Add(itr.Current);
				}

				foreach (var player in _timedOutPlayers)
				{
					// TODO:  Remove any buffers for this player
					_networkMediator.PlayerRepository.RemovePlayer(player);
				}
			}
		}
	}
}

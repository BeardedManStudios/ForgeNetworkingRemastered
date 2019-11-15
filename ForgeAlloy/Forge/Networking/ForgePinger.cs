using System;
using System.Threading;
using System.Threading.Tasks;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public class ForgePinger
	{
		public int PingInterval { get; set; } = 1000;
		private INetworkMediator _networkMediator;
		private SynchronizationContext _sourceSyncCtx;

		public void StartPinging(INetworkMediator networkMediator)
		{
			if (networkMediator.SocketFacade is ISocketServerFacade)
				throw new ArgumentException($"The ForgePinger can only be used on a client");
			_sourceSyncCtx = SynchronizationContext.Current;
			_networkMediator = networkMediator;
			Task.Run(PingAtInterval, _networkMediator.SocketFacade.CancellationSource.Token);
		}

		private void PingAtInterval()
		{
			while (!_networkMediator.SocketFacade.CancellationSource.IsCancellationRequested)
			{
				_sourceSyncCtx.Post(SendPingToServer, null);
				Thread.Sleep(PingInterval);
			}
		}

		private void SendPingToServer(object state)
		{
			_networkMediator.MessageBus.SendMessage(new ForgePingMessage(),
				_networkMediator.SocketFacade.ManagedSocket, _networkMediator.SocketFacade.ManagedSocket.EndPoint);
		}
	}
}

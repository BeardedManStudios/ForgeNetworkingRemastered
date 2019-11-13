using System.Net;
using System.Threading;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public abstract class ForgeUDPSocketContainerBase
	{
		public abstract ISocket ManagedSocket { get; }

		protected INetworkMediator networkMediator;
		public CancellationTokenSource CancellationSource { get; }
		protected SynchronizationContext synchronizationContext;

		public ForgeUDPSocketContainerBase()
		{
			synchronizationContext = SynchronizationContext.Current;
		}

		public virtual void ShutDown()
		{
			CancellationSource.Cancel();
			ManagedSocket.Close();
		}

		protected void ReadNetwork()
		{
			EndPoint readEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
			var buffer = new BMSByte();
			buffer.SetArraySize(2048);
			while (!CancellationSource.Token.IsCancellationRequested)
			{
				buffer.Clear();
				ManagedSocket.Receive(buffer, ref readEp);
				synchronizationContext.Post(SynchronizedMessageRead, new SocketContainerSynchronizationReadData
				{
					Buffer = buffer.CompressBytes(),
					Endpoint = readEp
				});
			}
		}

		protected void SynchronizedMessageRead(object state)
		{
			ProcessMessageRead((SocketContainerSynchronizationReadData)state);
		}

		protected virtual void ProcessMessageRead(SocketContainerSynchronizationReadData data)
		{
			INetPlayer player = networkMediator.PlayerRepository.GetPlayer(data.Endpoint);
			networkMediator.MessageBus.ReceiveMessageBuffer(ManagedSocket,
				player.EndPoint, data.Buffer);
		}
	}
}

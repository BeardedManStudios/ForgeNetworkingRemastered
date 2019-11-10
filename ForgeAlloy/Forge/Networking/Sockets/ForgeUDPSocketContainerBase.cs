using System.Net;
using System.Threading;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public abstract class ForgeUDPSocketContainerBase
	{
		public abstract ISocket ManagedSocket { get; }

		protected INetworkContainer netContainer;
		protected CancellationTokenSource readTokenSource;
		protected SynchronizationContext synchronizationContext;

		public ForgeUDPSocketContainerBase()
		{
			synchronizationContext = SynchronizationContext.Current;
		}

		public virtual void ShutDown()
		{
			readTokenSource.Cancel();
			ManagedSocket.Close();
		}

		protected void ReadNetwork()
		{
			EndPoint readEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
			var buffer = new BMSByte();
			buffer.SetArraySize(2048);
			while (!readTokenSource.Token.IsCancellationRequested)
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
			var data = (SocketContainerSynchronizationReadData)state;
			INetPlayer player = netContainer.PlayerRepository.GetPlayer(data.Endpoint);
			netContainer.MessageBus.ReceiveMessageBuffer(netContainer, ManagedSocket,
				player.Socket, data.Buffer);
		}
	}
}

using System;
using System.Net;
using System.Threading;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public abstract class ForgeUDPSocketFacadeBase
	{
		public abstract ISocket ManagedSocket { get; }

		protected INetworkMediator networkMediator;
		public CancellationTokenSource CancellationSource { get; protected set; }
		protected SynchronizationContext synchronizationContext;

		public ForgeUDPSocketFacadeBase()
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
			try
			{
				while (true)
				{
					CancellationSource.Token.ThrowIfCancellationRequested();
					buffer.Clear();
					ManagedSocket.Receive(buffer, ref readEp);
					synchronizationContext.Post(SynchronizedMessageRead, new SocketContainerSynchronizationReadData
					{
						Buffer = buffer.CompressBytes(),
						Endpoint = readEp
					});
				}
			}
			catch (OperationCanceledException) { }
		}

		protected void SynchronizedMessageRead(object state)
		{
			ProcessMessageRead((SocketContainerSynchronizationReadData)state);
		}

		protected virtual void ProcessMessageRead(SocketContainerSynchronizationReadData data)
		{
			// TODO:  Should check if player is banned
			INetPlayer player = networkMediator.PlayerRepository.GetPlayer(data.Endpoint);
			player.LastCommunication = DateTime.Now;
			ProcessPlayerMessageRead(player, data.Buffer);
		}

		protected virtual void ProcessPlayerMessageRead(INetPlayer player, byte[] buffer)
		{
			player.LastCommunication = DateTime.Now;
			networkMediator.MessageBus.ReceiveMessageBuffer(ManagedSocket,
				player.EndPoint, buffer);
		}
	}
}

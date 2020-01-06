using System;
using System.Net;
using System.Threading;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public abstract class ForgeUDPSocketFacadeBase
	{
		public abstract ISocket ManagedSocket { get; }

		protected INetworkMediator networkMediator;
		public CancellationTokenSource CancellationSource { get; protected set; }
		protected SynchronizationContext synchronizationContext;

		private BMSBytePool _readBufferPool = new BMSBytePool();

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
			EndPoint readEp = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 0);
			var buffer = new BMSByte();
			buffer.SetArraySize(2048);
			try
			{
				while (true)
				{
					CancellationSource.Token.ThrowIfCancellationRequested();
					buffer.Clear();
					ManagedSocket.Receive(buffer, ref readEp);
					BMSByte buff = _readBufferPool.Get(buffer.Size);
					buff.Clone(buffer);
					synchronizationContext.Post(SynchronizedMessageRead, new SocketContainerSynchronizationReadData
					{
						Buffer = buff,
						Endpoint = readEp
					});
				}
			}
			catch (OperationCanceledException) { }
		}

		protected void SynchronizedMessageRead(object state)
		{
			var s = (SocketContainerSynchronizationReadData)state;
			ProcessMessageRead(s);
			_readBufferPool.Release(s.Buffer);
		}

		protected abstract void ProcessMessageRead(SocketContainerSynchronizationReadData data);
	}
}

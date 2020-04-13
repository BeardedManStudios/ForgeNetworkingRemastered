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
					ProcessMessageRead(buffer, readEp);
				}
			}
			catch (OperationCanceledException)
			{
				networkMediator.EngineProxy.Logger.Log("Cancelling the background network read task");
			}
		}

		protected abstract void ProcessMessageRead(BMSByte buffer, EndPoint sender);
	}
}

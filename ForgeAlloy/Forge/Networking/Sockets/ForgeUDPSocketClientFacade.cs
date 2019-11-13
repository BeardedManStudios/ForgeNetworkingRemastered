using System;
using System.Threading.Tasks;
using Forge.Factory;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketClientFacade : ForgeUDPSocketContainerBase, ISocketClientFacade
	{
		public Guid NetPlayerId { get; set; }
		private readonly IClientSocket _socket;
		public override ISocket ManagedSocket => _socket;

		public ForgeUDPSocketClientFacade() : base()
		{
			_socket = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IClientSocket>();
		}

		public void StartClient(string address, ushort port, INetworkMediator netContainer)
		{
			this.networkMediator = netContainer;
			_socket.Connect(address, port);
			Task.Run(ReadNetwork, CancellationSource.Token);
		}
	}
}

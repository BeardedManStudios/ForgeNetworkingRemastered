using System;
using System.Threading.Tasks;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketClientContainer : ForgeUDPSocketContainerBase, ISocketClientContainer
	{
		public Guid NetPlayerId { get; set; }
		private readonly IClientSocket _socket;
		public override ISocket ManagedSocket => _socket;

		public ForgeUDPSocketClientContainer() : base()
		{
			_socket = ForgeTypeFactory.GetNew<IClientSocket>();
		}

		public void StartClient(string address, ushort port, INetworkContainer netContainer)
		{
			this.netContainer = netContainer;
			_socket.Connect(address, port);
			Task.Run(ReadNetwork, readTokenSource.Token);
		}
	}
}

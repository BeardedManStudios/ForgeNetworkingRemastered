using System.Threading.Tasks;
using Forge.Factory;
using Forge.Networking.Players;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketClientFacade : ForgeUDPSocketFacadeBase, ISocketClientFacade
	{
		public IPlayerSignature NetPlayerId { get; set; }
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

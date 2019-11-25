using System.Threading;
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

		public void StartClient(string address, ushort port, INetworkMediator netMediator)
		{
			networkMediator = netMediator;
			_socket.Connect(address, port);
			CancellationSource = new CancellationTokenSource();
			Task.Run(ReadNetwork, CancellationSource.Token);
			_socket.Send(_socket.EndPoint, new byte[1] { 1 }, 1);
		}

		protected override void ProcessMessageRead(SocketContainerSynchronizationReadData data)
		{
			networkMediator.MessageBus.ReceiveMessageBuffer(ManagedSocket,
				data.Endpoint, data.Buffer);
		}
	}
}

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Forge.Factory;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketClientFacade : ForgeUDPSocketFacadeBase, ISocketClientFacade
	{
		public IPlayerSignature NetPlayerId { get; set; }
		private readonly IClientSocket _socket;
		public override ISocket ManagedSocket => _socket;
		private ForgePinger _serverPing = new ForgePinger();

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
			BMSByte buffer = new BMSByte();
			buffer.Append(new byte[] { 1 });
			_socket.Send(_socket.EndPoint, buffer);
		}

		protected override void ProcessMessageRead(BMSByte buffer, EndPoint sender)
		{
			networkMediator.MessageBus.ReceiveMessageBuffer(ManagedSocket, sender, buffer);
		}

		public void Established(INetworkMediator netMediator)
		{
			_serverPing.StartPinging(netMediator);
		}
	}
}

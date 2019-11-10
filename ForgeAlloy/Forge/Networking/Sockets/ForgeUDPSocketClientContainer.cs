namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocketClientContainer : ISocketClientContainer
	{
		private readonly IClientSocket _socket;
		public ISocket ManagedSocket => _socket;

		public ForgeUDPSocketClientContainer()
		{
			_socket = ForgeTypeFactory.GetNew<IClientSocket>();
		}
	}
}

using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageClient : IMessageClient
	{
		public ISocket Socket { get; private set; }

		public ForgeMessageClient(ISocket socket)
		{
			Socket = socket;
		}

		public void Send(byte[] message)
		{
			Socket.Send(message, message.Length);
		}
	}
}

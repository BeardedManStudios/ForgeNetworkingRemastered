using Forge.Networking.Sockets;

namespace Forge.Networking.Messaging
{
	public interface IMessageClient
	{
		ISocket Socket { get; }

		void Send(byte[] message);
	}
}

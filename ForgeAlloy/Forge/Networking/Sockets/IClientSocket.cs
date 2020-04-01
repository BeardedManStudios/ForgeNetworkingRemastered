namespace Forge.Networking.Sockets
{
	public interface IClientSocket : ISocket
	{
		void Connect(string address, ushort port);
	}
}

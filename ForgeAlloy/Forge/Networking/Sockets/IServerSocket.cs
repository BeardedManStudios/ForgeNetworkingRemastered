namespace Forge.Networking.Sockets
{
	public interface IServerSocket : ISocket
	{
		void Listen(ushort port, int maxParallelConnections);
	}
}

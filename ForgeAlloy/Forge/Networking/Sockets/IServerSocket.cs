namespace Forge.Networking.Sockets
{
	public interface IServerSocket : ISocket
	{
		void Listen(string address, ushort port, int maxParallelConnections);
		ISocket AwaitAccept();
	}
}

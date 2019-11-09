namespace Forge.Networking.Sockets
{
	public interface IServerSocket
	{
		void Listen(string address, ushort port, int maxParallelConnections);
		ISocket AwaitAccept();
	}
}

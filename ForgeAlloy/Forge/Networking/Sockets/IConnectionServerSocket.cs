namespace Forge.Networking.Sockets
{
	public interface IConnectionServerSocket : IServerSocket
	{
		ISocket AwaitAccept();
	}
}

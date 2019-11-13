namespace Forge.Networking.Sockets
{
	public interface ISocketFacade
	{
		ISocket ManagedSocket { get; }
		void ShutDown();
	}
}

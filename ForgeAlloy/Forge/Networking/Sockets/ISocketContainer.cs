namespace Forge.Networking.Sockets
{
	public interface ISocketContainer
	{
		ISocket ManagedSocket { get; }
	}
}

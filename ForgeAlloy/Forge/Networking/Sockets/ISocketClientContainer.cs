namespace Forge.Networking.Sockets
{
	public interface ISocketClientContainer : ISocketContainer
	{
		void StartClient(string address, ushort port, INetworkContainer netContainer);
	}
}

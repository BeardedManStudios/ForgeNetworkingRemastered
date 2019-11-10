namespace Forge.Networking.Sockets
{
	public interface ISocketServerContainer : ISocketContainer
	{
		void StartServer(string address, ushort port, int maxPlayers, INetworkContainer netContainer);
	}
}

namespace Forge.Networking.Sockets
{
	public interface ISocketServerContainer : ISocketContainer
	{
		void StartServer(ushort port, int maxPlayers, INetworkContainer netContainer);
	}
}

using System.Net;

namespace Forge.Networking.Sockets
{
	public interface ISocketServerContainer : ISocketContainer
	{
		void StartServer(ushort port, int maxPlayers, INetworkContainer netContainer);
		void ChallengeSuccess(INetworkContainer netContainer, EndPoint endpoint);
	}
}

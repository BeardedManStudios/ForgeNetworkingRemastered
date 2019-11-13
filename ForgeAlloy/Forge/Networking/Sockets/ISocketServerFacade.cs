using System.Net;

namespace Forge.Networking.Sockets
{
	public interface ISocketServerFacade : ISocketFacade
	{
		void StartServer(ushort port, int maxPlayers, INetworkFacade netContainer);
		void ChallengeSuccess(INetworkFacade netContainer, EndPoint endpoint);
	}
}

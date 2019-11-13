using System.Net;

namespace Forge.Networking.Sockets
{
	public interface ISocketServerFacade : ISocketFacade
	{
		void StartServer(ushort port, int maxPlayers, INetworkMediator netContainer);
		void ChallengeSuccess(INetworkMediator netContainer, EndPoint endpoint);
	}
}

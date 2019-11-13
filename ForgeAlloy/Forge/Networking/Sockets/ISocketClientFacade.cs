using Forge.Networking.Players;

namespace Forge.Networking.Sockets
{
	public interface ISocketClientFacade : ISocketFacade
	{
		IPlayerSignature NetPlayerId { get; set; }
		void StartClient(string address, ushort port, INetworkMediator netContainer);
	}
}

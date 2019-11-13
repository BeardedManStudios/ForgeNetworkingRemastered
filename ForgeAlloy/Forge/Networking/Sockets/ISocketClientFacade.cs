using System;

namespace Forge.Networking.Sockets
{
	public interface ISocketClientFacade : ISocketFacade
	{
		Guid NetPlayerId { get; set; }
		void StartClient(string address, ushort port, INetworkMediator netContainer);
	}
}

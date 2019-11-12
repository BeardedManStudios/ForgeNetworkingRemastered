using System;

namespace Forge.Networking.Sockets
{
	public interface ISocketClientContainer : ISocketContainer
	{
		Guid NetPlayerId { get; set; }
		void StartClient(string address, ushort port, INetworkContainer netContainer);
	}
}

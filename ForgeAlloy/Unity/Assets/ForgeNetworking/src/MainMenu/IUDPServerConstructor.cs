using Forge.Engine;

namespace Forge.Networking.Unity
{
	public interface IUDPServerConstructor
	{
		ushort Port { get; }

		int MaxPlayers { get; }

		INetworkMediator CreateAndStartServer(IEngineContainer engineContainer);
	}
}

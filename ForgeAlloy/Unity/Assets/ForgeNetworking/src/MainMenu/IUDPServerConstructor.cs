using Forge.Engine;

namespace Forge.Networking.Unity
{
	public interface IUDPServerConstructor
	{
		ushort Port { get; }

		int MaxPlayers { get; }

		INetworkContainer CreateAndStartServer(IEngineContainer engineContainer);
	}
}

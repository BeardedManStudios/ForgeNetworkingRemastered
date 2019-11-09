using Forge.Engine;
using Forge.Networking.Players;

namespace Forge.Networking
{
	public interface INetworkContainer
	{
		IPlayerRepository PlayerRepository { get; }
		IEngineContainer EngineContainer { get; }
		void ChangeEngineContainer(IEngineContainer engineContainer);
	}
}

using Forge.Engine;
using Forge.Networking.Players;

namespace Forge.Networking
{
	public class ForgeNetworkContainer : INetworkContainer
	{
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineContainer EngineContainer { get; private set; }

		public ForgeNetworkContainer(IEngineContainer engineContainer)
		{
			PlayerRepository = ForgeTypeFactory.Get<IPlayerRepository>();
			ChangeEngineContainer(engineContainer);
		}

		public void ChangeEngineContainer(IEngineContainer engineContainer)
		{
			EngineContainer = engineContainer;
		}
	}
}

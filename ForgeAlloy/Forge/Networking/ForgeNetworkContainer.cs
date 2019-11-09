using Forge.Engine;
using Forge.Networking.Players;

namespace Forge.Networking
{
	public class ForgeNetworkContainer : INetworkContainer
	{
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineContainer EngineContainer { get; private set; }

		public ForgeNetworkContainer()
		{
			PlayerRepository = ForgeTypeFactory.Get<IPlayerRepository>();
		}

		public void ChangeEngineContainer(IEngineContainer engineContainer)
		{
			EngineContainer = engineContainer;
		}
	}
}

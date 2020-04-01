using Forge.Networking.Unity.UI;

namespace Forge.Networking.Unity.Menu
{
	public interface IMenuMediator
	{
		IUIButton<IMenuMediator> HostButton { get; }
		IUIButton<IMenuMediator> ConnectButton { get; }
		IUIInputField AddressInput { get; }
		IUIInputField PortInput { get; }
		IEngineFacade EngineFacade { get; }
		int MaxPlayers { get; }
	}
}

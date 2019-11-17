using Forge.Networking.Unity.UI;

namespace Forge.Networking.Unity
{
	public interface IMenuBridge
	{
		IUIButton HostButton { get; }
		IUIButton ConnectButton { get; }
		IUIInputField AddressInput { get; }
		IUIInputField PortInput { get; }

		void ConnectToServer();
		void HostServer();
	}
}

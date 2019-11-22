using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity.Menu
{
	public class MenuMediator : MonoBehaviour, IMenuMediator
	{
		[SerializeField]
		private int _maxPlayers = 32;
		public int MaxPlayers => _maxPlayers;

		[SerializeField]
		private HostButton _hostBtn;
		public IUIButton<IMenuMediator> HostButton => _hostBtn;

		[SerializeField]
		private ConnectButton _connectBtn;
		public IUIButton<IMenuMediator> ConnectButton => _connectBtn;

		[SerializeField]
		private UIInputField _addressField;
		public IUIInputField AddressInput => _addressField;

		[SerializeField]
		private UIInputField _portField;
		public IUIInputField PortInput => _portField;

		public IEngineFacade EngineFacade { get; private set; }

		private void Awake()
		{
			EngineFacade = FindObjectOfType<ForgeEngineFacade>();
			if (EngineFacade == null)
				throw new System.Exception($"Expected to find {typeof(ForgeEngineFacade)} but found nothing");
			HostButton.State = this;
			HostButton.Visible = true;
			ConnectButton.State = this;
			ConnectButton.Visible = true;
		}
	}
}

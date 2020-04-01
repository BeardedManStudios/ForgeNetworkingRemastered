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
		private HostButton _hostBtn = null;
		public IUIButton<IMenuMediator> HostButton => _hostBtn;

		[SerializeField]
		private ConnectButton _connectBtn = null;
		public IUIButton<IMenuMediator> ConnectButton => _connectBtn;

		[SerializeField]
		private UIInputField _addressField = null;
		public IUIInputField AddressInput => _addressField;

		[SerializeField]
		private UIInputField _portField = null;
		public IUIInputField PortInput => _portField;

		public IEngineFacade EngineFacade { get; private set; }

		private void Awake()
		{
			EngineFacade = FindObjectOfType<ForgeEngineFacade>();
			if (EngineFacade == null)
				throw new System.Exception($"Expected to find {typeof(ForgeEngineFacade)} in the scene but found nothing");
			HostButton.State = this;
			HostButton.Visible = true;
			ConnectButton.State = this;
			ConnectButton.Visible = true;
		}
	}
}

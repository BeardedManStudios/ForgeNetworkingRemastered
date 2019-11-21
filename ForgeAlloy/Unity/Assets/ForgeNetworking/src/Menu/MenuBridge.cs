using Forge.Factory;
using Forge.Networking.Unity.UI;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class MenuBridge : MonoBehaviour, IMenuBridge
	{
		[SerializeField]
		private int _maxPlayers = 32;

		[SerializeField]
		private UIButton _hostBtn;
		public IUIButton HostButton { get { return _hostBtn; } }

		[SerializeField]
		private UIButton _connectBtn;
		public IUIButton ConnectButton { get { return _connectBtn; } }

		[SerializeField]
		private UIInputField _addressField;
		public IUIInputField AddressInput { get { return _addressField; } }

		[SerializeField]
		private UIInputField _portField;
		public IUIInputField PortInput { get { return _portField; } }

		private IEngineFacade _engineFacade;
		private INetworkMediator _networkMediator;

		/// <summary>
		/// TODO: Move this to a better location
		/// </summary>
		private void Startup()
		{
			ForgeRegistration.Initialize();
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IEngineFacade, ForgeEngineFacade>();
			ForgeSerializationStrategy.Instance.AddSerializer<Vector3>(new Vector3Serializer());
		}

		private void Awake()
		{
			_engineFacade = FindObjectOfType<ForgeEngineFacade>();

			ThrowIfNull(_engineFacade);

			_hostBtn.RegisterCallback(HostServer);
			_connectBtn.RegisterCallback(ConnectToServer);
		}

		private void OnDestroy()
		{
			if (_networkMediator != null)
			{
				Debug.Log("Stopped Hosting");
				_networkMediator.SocketFacade.ShutDown();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.H))
			{
				HostServer();
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				ConnectToServer();
			}
		}

		public void HostServer()
		{
			if (_networkMediator == null)
			{
				Debug.Log("Hosting");
				var factory = AbstractFactory.Get<INetworkTypeFactory>();
				_networkMediator = factory.GetNew<INetworkMediator>();

				_networkMediator.ChangeEngineProxy(_engineFacade);

				//TODO: Catch exception if port is already being used (will not be caught in this function)
				_networkMediator.StartServer(ushort.Parse(PortInput.Text), _maxPlayers);
			}
			else
			{
				Debug.LogError("Already (Hosting | Connecting)");
			}
		}

		public void ConnectToServer()
		{
			if (_networkMediator != null)
			{
				Debug.LogError("Already (Hosting | Connecting)");
				return;
			}

			if (!string.IsNullOrEmpty(AddressInput.Text) && ushort.TryParse(PortInput.Text, out ushort port))
			{
				var factory = AbstractFactory.Get<INetworkTypeFactory>();
				_networkMediator = factory.GetNew<INetworkMediator>();

				_networkMediator.ChangeEngineProxy(_engineFacade);

				//TODO: Catch exception if connection fails
				_networkMediator.StartClient(AddressInput.Text, port);
			}
			else
			{
				Debug.LogError($"{ (string.IsNullOrEmpty(AddressInput.Text) ? "Host Address Not Provided" : "Port Invalid") }");
			}
		}

		private void ThrowIfNull<T>(T obj)
		{
			if (obj == null)
				throw new System.Exception($"Expected { obj } to implement { typeof(T) }");
		}
	}
}

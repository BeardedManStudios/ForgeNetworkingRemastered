using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class ForgeMain : MonoBehaviour
	{
		[SerializeField]
		private UnityEngineProxy _engineProxy;

		[SerializeField]
		private UDPServerConstructor _serverHostConstructor;

		[SerializeField]
		private UIButton _hostBtn;

		[SerializeField]
		private UIInputField _ip;

		[SerializeField]
		private UIInputField _port;

		[SerializeField]
		private UIButton _connectBtn;

		private INetworkMediator _networkMediator;

		private void Awake()
		{
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IUDPServerConstructor, UDPServerConstructor>();
			factory.Register<IEngineProxy, UnityEngineProxy>();

			ThrowIfNull(_engineProxy);
			ThrowIfNull(_serverHostConstructor);

			var engineProxy = _engineProxy;
			engineProxy.Prepare();

			_hostBtn.RegisterCallback(Host);
			_connectBtn.RegisterCallback(Connect);
		}

		private void OnDestroy()
		{
			if (_networkMediator != null)
			{
				_networkMediator.SocketFacade.ShutDown();
				Debug.Log("Stopped Hosting");
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.H))
			{
				Host();
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				Connect();
			}
		}

		public void Host()
		{
			if (_networkMediator == null)
			{
				_networkMediator = _serverHostConstructor.CreateAndStartServer(_engineProxy);
				Debug.Log("Hosting");
			}
			else
			{
				Debug.LogError("Already Hosting");
			}
		}

		public void Connect()
		{
			if (!string.IsNullOrEmpty(_ip.Text) && ushort.TryParse(_port.Text, out ushort port))
			{
				_networkMediator = _serverHostConstructor.ConnectToServer(_engineProxy, _ip.Text, port);
			}
			else
			{
				Debug.LogError($"{ (string.IsNullOrEmpty(_ip.Text) ? "Host Address Not Provided" : "Port Invalid") }");
			}
		}

		private void ThrowIfNull<T>(T obj)
		{
			if (obj == null)
				throw new System.Exception($"Expected { obj } to implement { typeof(T) }");
		}
	}
}

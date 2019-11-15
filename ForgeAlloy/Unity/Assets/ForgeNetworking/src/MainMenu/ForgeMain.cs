using System;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class ForgeMain : MonoBehaviour
	{
		[SerializeField]
		[Forge(typeof(IEngineProxy))]
		private UnityEngineProxy _engineProxy;

		[SerializeField]
		[Forge(typeof(IUDPServerConstructor))]
		private UDPServerConstructor _serverHostConstructor;

		[SerializeField]
		[Forge(typeof(IUIButton))]
		private UIButton _hostBtn;

		private INetworkMediator _networkMediator;

		private void Awake()
		{
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IUDPServerConstructor, UDPServerConstructor>();
			factory.Register<IEngineProxy, UnityEngineProxy>();

			ThrowIfNull(_engineProxy);
			ThrowIfNull(_serverHostConstructor);

			var engineProxy = _engineProxy as UnityEngineProxy;
			engineProxy.Prepare();

			_hostBtn.RegisterCallback(Host);
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
			//TODO: Connect
		}

		private void ThrowIfNull<T>(T obj)
		{
			if (obj == null)
				throw new System.Exception($"Expected { obj } to implement { typeof(T) }");
		}
	}
}

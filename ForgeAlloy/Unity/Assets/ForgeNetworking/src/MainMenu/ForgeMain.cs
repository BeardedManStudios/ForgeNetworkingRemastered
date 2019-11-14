using System;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class ForgeMain : MonoBehaviour
	{
		//TODO: Make this work
//		[Forge(typeof(IEngineContainer))]
//		private IEngineContainer _engineContainer;
//		[Forge(typeof(IUDPServerConstructor))]
//		private IUDPServerConstructor _serverHostConstructor;

		private IEngineContainer _engineContainer;
		private IUDPServerConstructor _serverHostConstructor;
		private INetworkMediator _networkMediator;
		private IUIButton _hostBtn;

		private void Awake()
		{
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IUDPServerConstructor, UDPServerConstructor>();
			factory.Register<IEngineContainer, UnityEngineContainer>();

			//TODO: Serializable field, these can be on different objects
			_engineContainer = GetComponent<IEngineContainer>();
			_serverHostConstructor = GetComponent<IUDPServerConstructor>();

			ThrowIfNull(_engineContainer);
			ThrowIfNull(_serverHostConstructor);

			var engineContainer = _engineContainer as UnityEngineContainer;
			engineContainer.Prepare();

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
				_networkMediator = _serverHostConstructor.CreateAndStartServer(_engineContainer);
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

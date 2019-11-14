using System;
using Forge.Engine;
using Forge.Factory;
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

		private void Awake()
		{
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IUDPServerConstructor, UDPServerConstructor>();
			factory.Register<IEngineContainer, UnityEngineContainer>();

			_engineContainer = GetComponent<IEngineContainer>();
			_serverHostConstructor = GetComponent<IUDPServerConstructor>();

			ThrowIfNull(_engineContainer);
			ThrowIfNull(_serverHostConstructor);

			var engineContainer = _engineContainer as UnityEngineContainer;
			engineContainer.Prepare();

			Host();
		}

		private void OnDestroy()
		{
			_networkMediator.SocketFacade.ShutDown();
		}

		public void Host()
		{
			_networkMediator = _serverHostConstructor.CreateAndStartServer(_engineContainer);
		}

		private void ThrowIfNull<T>(T obj)
		{
			if (obj == null)
				throw new System.Exception($"Expected { obj } to implement { typeof(T) }");
		}
	}
}

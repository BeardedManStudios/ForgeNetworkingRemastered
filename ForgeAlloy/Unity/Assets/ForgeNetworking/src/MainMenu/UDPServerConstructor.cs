using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace Forge.Networking.Unity
{
	public class UDPServerConstructor : MonoBehaviour, IUDPServerConstructor
	{
		[SerializeField]
		private ushort _port = 15937;
		public ushort Port => _port;

		[SerializeField]
		private int _maxPlayers = 32;
		public int MaxPlayers => _maxPlayers;

		[SerializeField]
		private Text _hostLabel;

		private ISocketServerFacade _socketFacade;

		private void Awake()
		{
			_hostLabel.text = $"(h) Host (127.0.0.1:{ _port })";
		}

		public INetworkMediator CreateAndStartServer(IEngineContainer engineContainer)
		{
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			_socketFacade = factory.GetNew<ISocketServerFacade>();
			var networkMediator = factory.GetNew<INetworkMediator>();

			networkMediator.ChangeSocketContainer(_socketFacade);
			networkMediator.ChangeEngineContainer(engineContainer);

			//TODO: Catch exception if port is already being used (will not be caught in this function)
			_socketFacade.StartServer(Port, MaxPlayers, networkMediator);

			return networkMediator;
		}
	}
}

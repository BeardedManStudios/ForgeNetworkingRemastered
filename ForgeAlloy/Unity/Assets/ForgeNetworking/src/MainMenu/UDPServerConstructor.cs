using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class UDPServerConstructor : MonoBehaviour, IUDPServerConstructor
	{
		[SerializeField]
		private ushort _port;
		public ushort Port => _port;

		[SerializeField]
		private int _maxPlayers;
		public int MaxPlayers => _maxPlayers;

		private ISocketServerFacade _socketFacade;

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

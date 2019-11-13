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

		public INetworkContainer CreateAndStartServer(IEngineContainer engineContainer)
		{
			var factory = AbstractFactory.Get<IFactory>();
			var socketServerContainer = factory.GetNew<ISocketServerContainer>();
			var networkContainer = factory.GetNew<INetworkContainer>();

			networkContainer.ChangeSocketContainer(socketServerContainer);
			networkContainer.ChangeEngineContainer(engineContainer);

			//TODO: Catch exception if port is already being used (will not be caught in this function)
			socketServerContainer.StartServer(Port, MaxPlayers, networkContainer);

			return networkContainer;
		}
	}
}

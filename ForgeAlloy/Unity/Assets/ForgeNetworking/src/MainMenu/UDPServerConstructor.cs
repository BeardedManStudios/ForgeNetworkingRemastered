using Forge.Engine;
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
			var socketServerContainer = ForgeTypeFactory.GetNew<ISocketServerContainer>();
			var networkContainer = ForgeTypeFactory.GetNew<INetworkContainer>();

			networkContainer.ChangeSocketContainer(socketServerContainer);
			networkContainer.ChangeEngineContainer(engineContainer);

			//TODO: Catch exception if port is already being used (will not be caught in this function)
			socketServerContainer.StartServer(Port, MaxPlayers, networkContainer);

			return networkContainer;
		}
	}
}

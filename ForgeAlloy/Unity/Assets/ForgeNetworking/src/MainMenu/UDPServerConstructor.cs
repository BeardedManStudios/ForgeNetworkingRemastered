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

		private void Awake()
		{
			_hostLabel.text = $"(h) Host (127.0.0.1:{ _port })";
		}

		public INetworkMediator CreateAndStartServer(IEngineProxy engineProxy)
		{
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			var networkMediator = factory.GetNew<INetworkMediator>();

			networkMediator.ChangeEngineProxy(engineProxy);

			//TODO: Catch exception if port is already being used (will not be caught in this function)
			networkMediator.StartServer(Port, MaxPlayers);

			return networkMediator;
		}
	}
}

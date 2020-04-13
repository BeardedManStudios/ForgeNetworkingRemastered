using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity
{
	public class ForgeEngineFacade : MonoBehaviour, IEngineFacade
	{
		private static readonly IForgeLogger _logger = new ForgeUnityLogger();
		public IForgeLogger Logger => _logger;

		[SerializeField]
		private string _sceneToLoad = "";
		public string CurrentMap => _sceneToLoad;

		private int _currentEntityId = 0;

		public INetworkMediator NetworkMediator { get; set; }
		private ISocketFacade _selfSocket => NetworkMediator.SocketFacade;

		// NOTE:  If the socket is a client socket, then _selfSocket.ManagedSocket.EndPoint
		// will always be the endpoint of the server for convenience
		private EndPoint _serverEndpoint => _selfSocket.ManagedSocket.EndPoint;

		[SerializeField]
		private PrefabManagerScriptableObject _prefabManager = null;
		public IPrefabManager PrefabManager => _prefabManager;

		public IMessageRepository NewClientMessageBuffer { get; private set; }
		public IEntityRepository EntityRepository { get; private set; }
		public bool IsServer => NetworkMediator.SocketFacade is ISocketServerFacade;

		public void NetworkingEstablished()
		{
			if (IsServer)
				ServerStarted();
			else
				ClientStarted();
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			ForgeRegistrations.Initialize();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			NewClientMessageBuffer = factory.GetNew<IMessageRepository>();
			EntityRepository = factory.GetNew<IEntityRepository>();
		}

		private void ServerStarted()
		{
			SceneManager.LoadScene(_sceneToLoad);
		}

		private void ClientStarted()
		{
			SceneManager.LoadScene(_sceneToLoad);
		}

		private void OnDestroy()
		{
			if (NetworkMediator != null)
				_selfSocket.ShutDown();
		}

		public int GetNewEntityId()
		{
			return _currentEntityId++;
		}
	}
}

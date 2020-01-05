using System.Net;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using Forge.Networking.Unity.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity
{
	public class ForgeEngineFacade : MonoBehaviour, IEngineFacade
	{
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
			Debug.Log("Network Established");

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
			NetworkMediator.PlayerRepository.onPlayerAddedSubscription += PlayerJoined;
			SceneManager.LoadScene(_sceneToLoad);
		}

		private void PlayerJoined(Players.INetPlayer player)
		{
			// TODO:  Go through all entities and their current pos/rot/scale and send it to the new player
		}

		private void ClientStarted()
		{
			NetworkMediator.MessageBus.SendReliableMessage(
					new MapLoadRequestMessage(),    // Request which scene to load
					_selfSocket.ManagedSocket,      // The socket we are going to send the message on
					_serverEndpoint);               // The receiver of the message (server)
		}

		private void OnDestroy()
		{
			if (NetworkMediator != null)
			{
				Debug.Log("Shutting down socket");
				_selfSocket.ShutDown();
			}
		}

		public int GetNewEntityId()
		{
			return _currentEntityId++;
		}
	}
}

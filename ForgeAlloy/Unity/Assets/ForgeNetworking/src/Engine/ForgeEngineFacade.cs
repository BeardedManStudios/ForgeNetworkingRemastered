using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;
using Forge.Networking.Unity.Messages;
using Forge.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity
{
	public class ForgeEngineFacade : MonoBehaviour, IEngineFacade
	{
		private int _currentEntityId = 0;
		private IEntityRepository _entityRepo;
		public IEntityRepository EntityRepository
		{
			get { return _entityRepo; }
			set { _entityRepo = value; }
		}

		public INetworkMediator NetworkMediator { get; set; }
		private ISocketFacade _selfSocket => NetworkMediator.SocketFacade;
		private bool _isServer => _selfSocket is ISocketServerFacade;

		// NOTE:  If the socket is a client socket, then _selfSocket.ManagedSocket.EndPoint
		// will always be the endpoint of the server for convenience
		private EndPoint _serverEndpoint => _selfSocket.ManagedSocket.EndPoint;

		[SerializeField]
		private PrefabManagerScriptableObject _prefabManager = null;
		public IPrefabManager PrefabManager => _prefabManager;

		public IMessageRepository NewClientMessageBuffer { get; private set; }

		public void NetworkingEstablished()
		{
			Debug.Log("Network Established");

			if (_isServer)
				ServerStarted();
			else
				ClientStarted();
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message, EndPoint sender)
		{

		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			ForgeRegistration.Initialize();
			ForgeSerializationStrategy.Instance.AddSerializer<Vector3>(new Vector3Serializer());
			ForgeSerializationStrategy.Instance.AddSerializer<Quaternion>(new QuaternionSerializer());
			NewClientMessageBuffer = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
		}

		private void Start()
		{
			_entityRepo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			_entityRepo.onEntityAdded += OnEntityAdded;
		}

		private void OnEntityAdded(IEntity entity)
		{

		}

		private void ServerStarted()
		{
			SceneManager.LoadScene("Cube");
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

using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;
using Forge.Networking.Sockets;
using Forge.Networking.Unity.Messages;
using Forge.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity
{
	public class ForgeEngineFacade : MonoBehaviour, IEngineFacade
	{
		private IEntityRepository _entityRepo;
		public IEntityRepository EntityRepository
		{
			get { return _entityRepo; }
			set { _entityRepo = value; }
		}

		public INetworkMediator NetworkMediator { get; set; }

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			ForgeRegistration.Initialize();
			ForgeSerializationStrategy.Instance.AddSerializer<Vector3>(new Vector3Serializer());
		}

		private void Start()
		{
			//TODO: Move this
			//_messageRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			_entityRepo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			_entityRepo.onEntityAdded += OnEntityAdded;
		}
		//private IMessageRepository _messageRepository;

		public void PlayerJoined(INetPlayer newPlayer)
		{
			//Helper for unity, to spawn cubes or do stuff for this new player
			Debug.Log("Player joined: " + newPlayer.Name);
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message, EndPoint sender)
		{
			//_messageRepository.AddMessage(message, sender);
		}

		private void OnEntityAdded(IEntity entity)
		{
			//			var messages = _messageRepository.GetAll();
			//
			//			//Be in a separate class that handles the messages exclusively
			//			// for no entities that have messages
			//			for (int i = 0; i < messages.Length; ++i)
			//			{
			//				var msg = (IEntityMessage)messages[i];
			//				if (msg.EntityId == entity.Id)
			//				{
			//					_messageRepository.RemoveMessage(msg);
			//					msg.Interpret(_networkContainer);
			//				}
			//			}
		}

		public void NetworkingEstablished()
		{
			Debug.Log("Network Established");

			if (NetworkMediator.SocketFacade is ISocketServerFacade)
				SceneManager.LoadScene("Cube");
			else
				NetworkMediator.MessageBus.SendReliableMessage(new MapLoadRequestMessage(),
					NetworkMediator.SocketFacade.ManagedSocket,
					NetworkMediator.SocketFacade.ManagedSocket.EndPoint);
		}

		private void OnDestroy()
		{
			if (NetworkMediator != null)
			{
				Debug.Log("Stopped Hosting");
				NetworkMediator.SocketFacade.ShutDown();
			}
		}
	}
}

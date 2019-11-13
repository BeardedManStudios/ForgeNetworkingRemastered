using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class UnityEngineContainer : MonoBehaviour, IEngineContainer
	{
		private IEntityRepository _entityRepo;
		public IEntityRepository EntityRepository
		{
			get { return _entityRepo; }
			set { _entityRepo = value; }
		}

		private INetworkContainer _networkContainer;
		private IMessageRepository _messageRepository;

		private void Start()
		{
			_messageRepository = AbstractFactory.Get<IFactory>().GetNew<IMessageRepository>();
			_entityRepo = AbstractFactory.Get<IFactory>().GetNew<IEntityRepository>();
			_entityRepo.onEntityAdded += OnEntityAdded;
		}

		public void PlayerJoined(INetPlayer newPlayer)
		{
			//Helper for unity, to spawn cubes or do stuff for this new player
			Debug.Log("Player joined: " + newPlayer.Name);
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message, EndPoint sender)
		{
			_messageRepository.AddMessage(message, sender);
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
	}
}

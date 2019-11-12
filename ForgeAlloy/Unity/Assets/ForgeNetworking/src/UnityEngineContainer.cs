using System.Collections.Generic;
using Forge.Engine;
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

		private void Awake()
		{
			_messageRepository = ForgeTypeFactory.GetNew<IMessageRepository>();
			_entityRepo.onEntityAdded += OnEntityAdded;
		}

		public void PlayerJoined(INetPlayer newPlayer)
		{
			//Helper for unity, to spawn cubes or do stuff for this new player
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message)
		{
			_messageRepository.AddMessage(message);
		}

		private void OnEntityAdded(IEntity entity)
		{
			var messages = _messageRepository.GetAll();

			//Be in a separate class that handles the messages exclusively
			// for no entities that have messages
			for (int i = 0; i < messages.Length; ++i)
			{
				var msg = (IEntityMessage)messages[i];
				if (msg.EntityId == entity.Id)
				{
					_messageRepository.RemoveMessage(msg);
					msg.Interpret(_networkContainer);
				}
			}
		}
	}
}

using Forge.Engine;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;
using UnityEngine;

namespace Forge.Networking.Runtime
{
	public class RuntimeEngineContainer : MonoBehaviour, IEngineContainer
	{
		public IEntityRepository EntityRepository
		{
			get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException();
		}

		public IEntity FindEntityWithId(int id)
		{
			throw new System.NotImplementedException();
		}

		public void PlayerJoined(INetPlayer newPlayer)
		{
			throw new System.NotImplementedException();
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message)
		{
			throw new System.NotImplementedException();
		}
	}
}

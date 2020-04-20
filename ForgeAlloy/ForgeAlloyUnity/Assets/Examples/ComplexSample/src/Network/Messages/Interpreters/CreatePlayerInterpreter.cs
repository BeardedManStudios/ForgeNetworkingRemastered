using System.Net;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Unity;

namespace Puzzle.Networking.Messages.Interpreters
{
	public class CreatePlayerInterpreter : IMessageInterpreter
	{
		public static CreatePlayerInterpreter Instance { get; private set; } = new CreatePlayerInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;
		public event OwnedUnityEntityEvent onPlayerCreated;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var m = (CreatePlayerMessage)message;
			bool isMe = netMediator.SocketFacade.NetPlayerId.Equals(m.OwningPlayer);
			if (!isMe)
				m.PrefabId = m.ProxyPrefabId;
			IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;
			var e = EntitySpawner.SpawnEntityFromMessage(engine, m);
			onPlayerCreated?.Invoke(e, m.OwningPlayer);
		}
	}
}

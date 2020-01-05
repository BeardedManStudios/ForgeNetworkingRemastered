using System.Net;
using Forge.Networking.Messaging;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SendEntitiesToNewPlayerInterpreter : IMessageInterpreter
	{
		public static SendEntitiesToNewPlayerInterpreter Instance { get; private set; } = new SendEntitiesToNewPlayerInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var m = (SendEntitiesToNewPlayerMessage)message;
			IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;
			for (int i = 0; i < m.EntityCount; i++)
				EntitySpawnner.SpawnEntityFromData(engine, m.Ids[i], m.PrefabIds[i], m.Positions[i], m.Rotations[i], m.Scales[i]);
		}
	}
}

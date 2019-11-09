using Forge.Engine;
using Forge.Networking.Messaging.Messages;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeEntityMessageInterpreter : IMessageInterpreter
	{
		public void Interpret(INetworkContainer netHost, IMessage message)
		{
			var eMessage = (ForgeEntityMessage)message;
			IEntity entity = netHost.EngineContainer.FindEntityWithId(eMessage.EntityId);
			entity.ProcessNetworkMessage(message);
		}
	}
}

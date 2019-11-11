using Forge.Engine;
using Forge.Networking.Messaging.Messages;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeEntityMessageInterpreter : IMessageInterpreter
	{
		public void Interpret(INetworkContainer netContainer, IMessage message)
		{
			var eMessage = (IEntityMessage)message;
			try
			{
				IEntity entity = netContainer.EngineContainer.EntityRepository.GetEntityById(eMessage.EntityId);
				entity.ProcessNetworkMessage(eMessage);
			}
			catch (EngineEntityNotFoundException)
			{
				netContainer.EngineContainer.ProcessUnavailableEntityMessage(eMessage);
			}
		}
	}
}

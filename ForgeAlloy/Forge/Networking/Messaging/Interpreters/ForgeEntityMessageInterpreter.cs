using System.Net;
using Forge.Engine;
using Forge.Networking.Messaging.Messages;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeEntityMessageInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => true;

		public void Interpret(INetworkContainer netContainer, EndPoint sender, IMessage message)
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

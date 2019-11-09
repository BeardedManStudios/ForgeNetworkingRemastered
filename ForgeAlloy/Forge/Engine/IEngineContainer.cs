using Forge.Networking.Messaging.Messages;

namespace Forge.Engine
{
	public interface IEngineContainer
	{
		IEntityRepository EntityRepository { get; set; }
		IEntity FindEntityWithId(int id);
		void ProcessUnavailableEntityMessage(IEntityMessage message);
	}
}

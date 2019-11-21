using Forge.Engine;

namespace Forge.Networking.Messaging.Messages
{
	public interface IEntityMessage : IMessage
	{
		int EntityId { get; set; }
		void ProcessUsing(IEntity entity);
	}
}

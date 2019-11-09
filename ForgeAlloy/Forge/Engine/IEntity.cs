using Forge.Networking.Messaging.Messages;

namespace Forge.Engine
{
	public interface IEntity
	{
		int Id { get; set; }
		void ProcessNetworkMessage(IEntityMessage message);
	}
}

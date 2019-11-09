using Forge.Networking.Messaging;

namespace Forge.Engine
{
	public interface IEntity
	{
		int Id { get; set; }
		void ProcessNetworkMessage(IMessage message);
	}
}

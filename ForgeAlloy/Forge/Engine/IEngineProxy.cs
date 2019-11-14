using System.Net;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;

namespace Forge.Engine
{
	public interface IEngineProxy
	{
		IEntityRepository EntityRepository { get; set; }
		void ProcessUnavailableEntityMessage(IEntityMessage message, EndPoint sender);
		void PlayerJoined(INetPlayer newPlayer);
	}
}

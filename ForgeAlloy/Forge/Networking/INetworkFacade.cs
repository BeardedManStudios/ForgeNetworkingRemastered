using Forge.Engine;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public interface INetworkMediator
	{
		IPlayerRepository PlayerRepository { get; }
		IEngineProxy EngineContainer { get; }
		IMessageBus MessageBus { get; }
		ISocketFacade SocketFacade { get; }
		void ChangeEngineFacade(IEngineProxy engineContainer);
		void StartServer(ushort port, int maxPlayers);
		void StartClient(string hostAddress, ushort port);
		void SendMessage(IMessage message, IPlayerSignature playerId);
		void SendMessage(IMessage message, INetPlayer player);
	}
}

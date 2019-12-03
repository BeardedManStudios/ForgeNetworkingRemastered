using Forge.Engine;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public interface INetworkMediator
	{
		int PlayerTimeout { get; }
		IPlayerRepository PlayerRepository { get; }
		IEngineProxy EngineProxy { get; }
		IMessageBus MessageBus { get; }
		ISocketFacade SocketFacade { get; }
		void ChangeEngineProxy(IEngineProxy engineProxy);
		void StartServer(ushort port, int maxPlayers);
		void StartClient(string hostAddress, ushort port);
		void SendMessage(IMessage message, IPlayerSignature playerId);
		void SendMessage(IMessage message, INetPlayer player);
	}
}

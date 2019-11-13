using System;
using Forge.Engine;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public interface INetworkMediator
	{
		IPlayerRepository PlayerRepository { get; }
		IEngineContainer EngineContainer { get; }
		IMessageBus MessageBus { get; }
		ISocketFacade SocketFacade { get; }
		void ChangeEngineContainer(IEngineContainer engineContainer);
		void ChangeSocketContainer(ISocketFacade socketContainer);
		void SendMessage(IMessage message, Guid playerId);
		void SendMessage(IMessage message, INetPlayer player);
	}
}

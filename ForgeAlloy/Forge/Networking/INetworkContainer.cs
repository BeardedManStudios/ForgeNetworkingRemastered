using System;
using Forge.Engine;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public interface INetworkContainer
	{
		IPlayerRepository PlayerRepository { get; }
		IEngineContainer EngineContainer { get; }
		IMessageBus MessageBus { get; }
		ISocketContainer SocketContainer { get; }
		void ChangeEngineContainer(IEngineContainer engineContainer);
		void ChangeSocketContainer(ISocketContainer socketContainer);
		void SendMessage(IMessage message, Guid playerId);
		void SendMessage(IMessage message, INetPlayer player);
	}
}

using System;
using Forge.Engine;
using Forge.Networking.Messaging;
using Forge.Networking.Players;

namespace Forge.Networking
{
	public interface INetworkContainer
	{
		IPlayerRepository PlayerRepository { get; }
		IEngineContainer EngineContainer { get; }
		IMessageBus MessageBus { get; }
		INetPlayer SelfPlayer { get; }
		void ChangeEngineContainer(IEngineContainer engineContainer);
		void SendMessage(IMessage message, Guid playerId);
		void SendMessage(IMessage message, INetPlayer player);
	}
}

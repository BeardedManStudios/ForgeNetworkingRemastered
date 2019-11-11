using System;
using Forge.Engine;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public class ForgeNetworkContainer : INetworkContainer
	{
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineContainer EngineContainer { get; private set; }
		public IMessageBus MessageBus { get; private set; }
		public ISocketContainer SocketContainer { get; private set; }

		public ForgeNetworkContainer()
		{
			PlayerRepository = ForgeTypeFactory.GetNew<IPlayerRepository>();
			MessageBus = ForgeTypeFactory.GetNew<IMessageBus>();
		}

		public void ChangeEngineContainer(IEngineContainer engineContainer)
		{
			EngineContainer = engineContainer;
		}

		public void ChangeSocketContainer(ISocketContainer socketContainer)
		{
			SocketContainer = socketContainer;
		}

		public void SendMessage(IMessage message, Guid playerId)
		{
			INetPlayer player = PlayerRepository.GetPlayer(playerId);
			SendMessage(message, player);
		}

		public void SendMessage(IMessage message, INetPlayer player)
		{
			MessageBus.SendMessage(message, SocketContainer.ManagedSocket, player.Socket);
		}
	}
}

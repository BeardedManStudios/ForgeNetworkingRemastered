using System;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public class ForgeNetworkFacade : INetworkFacade
	{
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineContainer EngineContainer { get; private set; }
		public IMessageBus MessageBus { get; private set; }
		public ISocketFacade SocketContainer { get; private set; }

		public ForgeNetworkFacade()
		{
			PlayerRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			MessageBus = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBus>();
		}

		public void ChangeEngineContainer(IEngineContainer engineContainer)
		{
			EngineContainer = engineContainer;
		}

		public void ChangeSocketContainer(ISocketFacade socketContainer)
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
			MessageBus.SendMessage(message, SocketContainer.ManagedSocket, player.EndPoint);
		}
	}
}

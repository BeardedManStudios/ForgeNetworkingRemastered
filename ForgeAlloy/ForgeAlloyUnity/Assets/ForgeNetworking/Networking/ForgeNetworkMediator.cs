using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Networking
{
	public class ForgeNetworkMediator : INetworkMediator
	{
		public int PlayerTimeout => _timeoutBridge.TimeoutMilliseconds;
		public IPlayerRepository PlayerRepository { get; private set; }
		public IEngineProxy EngineProxy { get; private set; }
		public IMessageBus MessageBus { get; private set; }
		public ISocketFacade SocketFacade { get; private set; }
		public bool IsClient => SocketFacade is ISocketClientFacade;
		public bool IsServer => SocketFacade is ISocketServerFacade;

		private readonly IPlayerTimeoutBridge _timeoutBridge;

		public ForgeNetworkMediator()
		{
			PlayerRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			MessageBus = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBus>();
			_timeoutBridge = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerTimeoutBridge>();
		}

		public void ChangeEngineProxy(IEngineProxy engineProxy)
		{
			EngineProxy = engineProxy;
		}

		public void StartServer(ushort port, int maxPlayers)
		{
			var server = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			SocketFacade = server;
			server.StartServer(port, maxPlayers, this);
			_timeoutBridge.StartWatching(this);
			EngineProxy.NetworkingEstablished();
			MessageBus.SetMediator(this);
		}

		public void StartClient(string hostAddress, ushort port)
		{
			var client = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketClientFacade>();
			SocketFacade = client;
			client.StartClient(hostAddress, port, this);
			MessageBus.SetMediator(this);
		}

		public void SendMessage(IMessage message)
		{
			if (SocketFacade is ISocketServerFacade)
			{
				var itr = PlayerRepository.GetEnumerator();
				while (itr.MoveNext())
				{
					if (itr.Current != null)
						MessageBus.SendMessage(message, SocketFacade.ManagedSocket, itr.Current.EndPoint);
				}
			}
			else
				MessageBus.SendMessage(message, SocketFacade.ManagedSocket, SocketFacade.ManagedSocket.EndPoint);
		}

		public void SendMessage(IMessage message, INetPlayer player)
		{
			MessageBus.SendMessage(message, SocketFacade.ManagedSocket, player.EndPoint);
		}

		public void SendMessage(IMessage message, EndPoint endpoint)
		{
			MessageBus.SendMessage(message, SocketFacade.ManagedSocket, endpoint);
		}

		public void SendReliableMessage(IMessage message)
		{
			if (SocketFacade is ISocketServerFacade)
			{
				var itr = PlayerRepository.GetEnumerator();
				while (itr.MoveNext())
				{
					if (itr.Current != null)
						MessageBus.SendReliableMessage(message, SocketFacade.ManagedSocket, itr.Current.EndPoint);
				}
			}
			else
				MessageBus.SendReliableMessage(message, SocketFacade.ManagedSocket, SocketFacade.ManagedSocket.EndPoint);
		}

		public void SendReliableMessage(IMessage message, INetPlayer player)
		{
			MessageBus.SendMessage(message, SocketFacade.ManagedSocket, player.EndPoint);
		}

		public void SendReliableMessage(IMessage message, EndPoint endpoint)
		{
			MessageBus.SendMessage(message, SocketFacade.ManagedSocket, endpoint);
		}
	}
}

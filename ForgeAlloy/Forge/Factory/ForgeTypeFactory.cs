using Forge.Engine;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge.Factory
{
	public class ForgeTypeFactory : TypeFactory, INetworkTypeFactory
	{
		public override void PrimeRegistry()
		{
			Register<IPlayerRepository, ForgePlayerRepository>();
			Register<INetworkMediator, ForgeNetworkFacade>();
			Register<IEntityRepository, ForgeEntityRepository>();
			Register<IMessageReceipt, ForgeMessageReceipt>();
			Register<IMessageBus, ForgeMessageBus>();
			Register<IMessageRepository, ForgeMessageRepository>();
			Register<IMessagePage, ForgeMessagePage>();
			Register<IPagenatedMessage, ForgePagenatedMessage>();
			Register<IMessageDestructor, ForgeMessageDestructor>();
			Register<IMessageConstructor, ForgeMessageConstructor>();
			Register<IMessageBufferInterpreter, ForgeMessageBufferInterpreter>();
			Register<ISocket, ForgeUDPSocket>();
			Register<IServerSocket, ForgeUDPSocket>();
			Register<IClientSocket, ForgeUDPSocket>();
			Register<ISocketServerFacade, ForgeUDPSocketServerFacade>();
			Register<INetPlayer, ForgePlayer>();
			Register<IEntityMessage, ForgeEntityMessage>();
			Register<IChallengeMessage, ForgeConnectChallengeMessage>();
			Register<IChallengeResponseMessage, ForgeConnectChallengeResponseMessage>();
			Register<IMessageRepeater, ForgeMessageRepeater>();
		}
	}
}

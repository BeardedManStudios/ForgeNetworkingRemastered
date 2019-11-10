using Forge.Engine;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;

namespace Forge
{
	public static class ForgeRegistration
	{
		public static void Initialize()
		{
			// Containers and repositories
			ForgeTypeFactory.Register<IPlayerRepository, ForgePlayerRepository>();
			ForgeTypeFactory.Register<INetworkContainer, ForgeNetworkContainer>();
			ForgeTypeFactory.Register<IEntityRepository, ForgeEntityRepository>();

			// Messaging
			ForgeTypeFactory.Register<IMessageReceipt, ForgeMessageReceipt>();
			ForgeTypeFactory.Register<IMessageBus, ForgeMessageBus>();
			ForgeTypeFactory.Register<IMessageRepository, ForgeMessageRepository>();

			// Message pagination
			ForgeTypeFactory.Register<IMessagePage, ForgeMessagePage>();
			ForgeTypeFactory.Register<IPagenatedMessage, ForgePagenatedMessage>();
			ForgeTypeFactory.Register<IMessageDestructor, ForgeMessageDestructor>();
			ForgeTypeFactory.Register<IMessageConstructor, ForgeMessageConstructor>();
			ForgeTypeFactory.Register<IMessageBufferInterpreter, ForgeMessageBufferInterpreter>();

			// Sockets
			ForgeTypeFactory.Register<ISocket, ForgeUDPSocket>();
			ForgeTypeFactory.Register<IServerSocket, ForgeUDPSocket>();
			ForgeTypeFactory.Register<IClientSocket, ForgeUDPSocket>();
			ForgeTypeFactory.Register<ISocketServerContainer, ForgeUDPSocketServerContainer>();
			ForgeTypeFactory.Register<INetPlayer, ForgePlayer>();

			// Custom messages
			ForgeTypeFactory.Register<IEntityMessage, ForgeEntityMessage>();

			// Add the message registry here for the codes
			ForgeMessageCodes.Register<ForgeReceiptAcknowledgement>();
			ForgeMessageCodes.Register<ForgeEntityMessage>();
		}

		public static void Teardown()
		{
			ForgeTypeFactory.Clear();
			ForgeMessageCodes.Clear();
		}
	}
}

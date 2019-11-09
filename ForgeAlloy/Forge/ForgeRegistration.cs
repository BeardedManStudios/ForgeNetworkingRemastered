using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Players;

namespace Forge
{
	public static class ForgeRegistration
	{
		public static void Initialize()
		{
			ForgeTypeFactory.Register<IPlayerRepository, ForgePlayerRepository>();
			ForgeTypeFactory.Register<IMessageReceipt, ForgeMessageReceipt>();
			ForgeTypeFactory.Register<IMessageBus, ForgeMessageBus>();
			ForgeTypeFactory.Register<IMessageRepository, ForgeMessageRepository>();
			ForgeTypeFactory.Register<INetworkContainer, ForgeNetworkContainer>();

			// Custom messages
			ForgeTypeFactory.Register<IEntityMessage, ForgeEntityMessage>();

			// Add the message registry here for the codes
			ForgeMessageCodes.Register<ForgeEntityMessage>();
		}

		public static void Teardown()
		{
			ForgeTypeFactory.Clear();
			ForgeMessageCodes.Clear();
		}
	}
}

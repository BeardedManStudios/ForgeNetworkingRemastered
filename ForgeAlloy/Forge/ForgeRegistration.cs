using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Messaging.Paging;
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

			// Message pagination
			ForgeTypeFactory.Register<IMessagePage, ForgeMessagePage>();
			ForgeTypeFactory.Register<IPagenatedMessage, ForgePagenatedMessage>();
			ForgeTypeFactory.Register<IMessageDestructor, ForgeMessageDestructor>();
			ForgeTypeFactory.Register<IMessageConstructor, ForgeMessageConstructor>();

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

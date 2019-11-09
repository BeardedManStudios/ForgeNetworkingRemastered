using Forge.Networking.Messaging;
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

			// TODO:  Add the message registry here for the codes
			//ForgeMessageCodes.Register<SendPlayerName>(ForgeMessageCodes.SEND_NAME_MESSAGE);
		}

		public static void Teardown()
		{
			ForgeTypeFactory.Clear();
			ForgeMessageCodes.Clear();
		}
	}
}

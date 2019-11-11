using Forge.Engine;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;
using Forge.Serialization;
using Forge.Serialization.Serializers;

namespace Forge
{
	public static class ForgeRegistration
	{
		public static void Initialize()
		{
			RegisterContainers();
			RegisterMessaging();
			RegisterMessagePagination();
			RegisterSockets();
			RegisterCustomMessages();
			RegisterMessageCodes();
			SetupSerializers();
		}

		public static void Teardown()
		{
			ForgeTypeFactory.Clear();
			ForgeMessageCodes.Clear();
			ForgeSerializationContainer.Instance.Clear();
		}

		private static void RegisterContainers()
		{
			ForgeTypeFactory.Register<IPlayerRepository, ForgePlayerRepository>();
			ForgeTypeFactory.Register<INetworkContainer, ForgeNetworkContainer>();
			ForgeTypeFactory.Register<IEntityRepository, ForgeEntityRepository>();
		}

		private static void RegisterMessaging()
		{
			ForgeTypeFactory.Register<IMessageReceipt, ForgeMessageReceipt>();
			ForgeTypeFactory.Register<IMessageBus, ForgeMessageBus>();
			ForgeTypeFactory.Register<IMessageRepository, ForgeMessageRepository>();
		}

		private static void RegisterMessagePagination()
		{
			ForgeTypeFactory.Register<IMessagePage, ForgeMessagePage>();
			ForgeTypeFactory.Register<IPagenatedMessage, ForgePagenatedMessage>();
			ForgeTypeFactory.Register<IMessageDestructor, ForgeMessageDestructor>();
			ForgeTypeFactory.Register<IMessageConstructor, ForgeMessageConstructor>();
			ForgeTypeFactory.Register<IMessageBufferInterpreter, ForgeMessageBufferInterpreter>();
		}

		private static void RegisterSockets()
		{
			ForgeTypeFactory.Register<ISocket, ForgeUDPSocket>();
			ForgeTypeFactory.Register<IServerSocket, ForgeUDPSocket>();
			ForgeTypeFactory.Register<IClientSocket, ForgeUDPSocket>();
			ForgeTypeFactory.Register<ISocketServerContainer, ForgeUDPSocketServerContainer>();
			ForgeTypeFactory.Register<INetPlayer, ForgePlayer>();
		}

		private static void RegisterCustomMessages()
		{
			ForgeTypeFactory.Register<IEntityMessage, ForgeEntityMessage>();
		}

		private static void RegisterMessageCodes()
		{
			ForgeMessageCodes.Register<ForgeReceiptAcknowledgement>();
			ForgeMessageCodes.Register<ForgeEntityMessage>();
		}

		private static void SetupSerializers()
		{
			ForgeSerializationContainer.Instance.AddSerializer<byte>(new ByteSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<sbyte>(new SByteSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<short>(new ShortSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<ushort>(new UShortSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<int>(new IntSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<uint>(new UIntSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<long>(new LongSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<ulong>(new ULongSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<float>(new FloatSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<double>(new DoubleSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<bool>(new BoolSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<string>(new StringSerializer());
			ForgeSerializationContainer.Instance.AddSerializer<byte[]>(new ByteArraySerializer());
		}
	}
}

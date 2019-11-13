using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Messages;
using Forge.Serialization;
using Forge.Serialization.Serializers;

namespace Forge
{
	public static class ForgeRegistration
	{
		public static void Initialize()
		{
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			RegisterMessageCodes();
			SetupSerializers();
		}

		public static void Teardown()
		{
			AbstractFactory.Clear();
			ForgeMessageCodes.Clear();
			ForgeSerializationContainer.Instance.Clear();
		}

		private static void RegisterMessageCodes()
		{
			ForgeMessageCodes.Register<ForgeReceiptAcknowledgement>();
			ForgeMessageCodes.Register<ForgeEntityMessage>();
			ForgeMessageCodes.Register<ForgeRequestIdentityMessage>();
			ForgeMessageCodes.Register<ForgeNetworkIdentityMessage>();
			ForgeMessageCodes.Register<ForgeReadyForEngineMessage>();
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

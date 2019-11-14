using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Serialization;
using Forge.Serialization.Serializers;
using Forge.ServerRegistry.DataStructures;
using Forge.ServerRegistry.Serializers;

namespace Forge
{
	public static class ForgeRegistration
	{
		public static void Initialize()
		{
			AbstractFactory.Register<INetworkTypeFactory, ForgeTypeFactory>();
			ForgeMessageCodes.Register();
			SetupSerializers();
		}

		public static void Teardown()
		{
			AbstractFactory.Clear();
			ForgeMessageCodes.Clear();
			ForgeSerializationStrategy.Instance.Clear();
		}

		private static void SetupSerializers()
		{
			ForgeSerializationStrategy.Instance.AddSerializer<byte>(new ByteSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<sbyte>(new SByteSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<short>(new ShortSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<ushort>(new UShortSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<int>(new IntSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<uint>(new UIntSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<long>(new LongSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<ulong>(new ULongSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<float>(new FloatSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<double>(new DoubleSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<bool>(new BoolSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<string>(new StringSerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<byte[]>(new ByteArraySerializer());
			ForgeSerializationStrategy.Instance.AddSerializer<IMessageReceiptSignature>(new ForgeSignatureSerializer<IMessageReceiptSignature>());
			ForgeSerializationStrategy.Instance.AddSerializer<IPlayerSignature>(new ForgeSignatureSerializer<IPlayerSignature>());

			ForgeSerializationStrategy.Instance.AddSerializer<ServerListingEntry[]>(new ServerListingEntryArraySerializer());
		}
	}
}

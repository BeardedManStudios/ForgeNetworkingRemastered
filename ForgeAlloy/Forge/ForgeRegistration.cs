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
			ForgeSerialization.Instance.Clear();
		}

		private static void SetupSerializers()
		{
			ForgeSerialization.Instance.AddSerializer<byte>(new ByteSerializer());
			ForgeSerialization.Instance.AddSerializer<sbyte>(new SByteSerializer());
			ForgeSerialization.Instance.AddSerializer<short>(new ShortSerializer());
			ForgeSerialization.Instance.AddSerializer<ushort>(new UShortSerializer());
			ForgeSerialization.Instance.AddSerializer<int>(new IntSerializer());
			ForgeSerialization.Instance.AddSerializer<uint>(new UIntSerializer());
			ForgeSerialization.Instance.AddSerializer<long>(new LongSerializer());
			ForgeSerialization.Instance.AddSerializer<ulong>(new ULongSerializer());
			ForgeSerialization.Instance.AddSerializer<float>(new FloatSerializer());
			ForgeSerialization.Instance.AddSerializer<double>(new DoubleSerializer());
			ForgeSerialization.Instance.AddSerializer<bool>(new BoolSerializer());
			ForgeSerialization.Instance.AddSerializer<string>(new StringSerializer());
			ForgeSerialization.Instance.AddSerializer<byte[]>(new ByteArraySerializer());
			ForgeSerialization.Instance.AddSerializer<IMessageReceiptSignature>(new ForgeSignatureSerializer<IMessageReceiptSignature>());
			ForgeSerialization.Instance.AddSerializer<IPlayerSignature>(new ForgeSignatureSerializer<IPlayerSignature>());

			ForgeSerialization.Instance.AddSerializer<ServerListingEntry[]>(new ServerListingEntryArraySerializer());
		}
	}
}

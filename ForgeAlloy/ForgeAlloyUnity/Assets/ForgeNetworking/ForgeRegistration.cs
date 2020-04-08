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
			AbstractFactory.Register<INetworkTypeFactory, ForgeNetworkingTypeFactory>();
			ForgeMessageCodes.Register();
			SetupSerializers();
		}

		public static void Teardown()
		{
			AbstractFactory.Clear();
			ForgeMessageCodes.Clear();
			ForgeSerializer.Instance.Clear();
		}

		private static void SetupSerializers()
		{
			ForgeSerializer.Instance.AddSerializer<byte>(new ByteSerializer());
			ForgeSerializer.Instance.AddSerializer<sbyte>(new SByteSerializer());
			ForgeSerializer.Instance.AddSerializer<short>(new ShortSerializer());
			ForgeSerializer.Instance.AddSerializer<ushort>(new UShortSerializer());
			ForgeSerializer.Instance.AddSerializer<int>(new IntSerializer());
			ForgeSerializer.Instance.AddSerializer<uint>(new UIntSerializer());
			ForgeSerializer.Instance.AddSerializer<long>(new LongSerializer());
			ForgeSerializer.Instance.AddSerializer<ulong>(new ULongSerializer());
			ForgeSerializer.Instance.AddSerializer<float>(new FloatSerializer());
			ForgeSerializer.Instance.AddSerializer<double>(new DoubleSerializer());
			ForgeSerializer.Instance.AddSerializer<bool>(new BoolSerializer());
			ForgeSerializer.Instance.AddSerializer<string>(new StringSerializer());
			ForgeSerializer.Instance.AddSerializer<byte[]>(new ByteArraySerializer());
			ForgeSerializer.Instance.AddSerializer<IMessageReceiptSignature>(new ForgeSignatureSerializer<IMessageReceiptSignature>());
			ForgeSerializer.Instance.AddSerializer<IPlayerSignature>(new ForgeSignatureSerializer<IPlayerSignature>());

			ForgeSerializer.Instance.AddSerializer<ServerListingEntry[]>(new ServerListingEntryArraySerializer());
		}
	}
}

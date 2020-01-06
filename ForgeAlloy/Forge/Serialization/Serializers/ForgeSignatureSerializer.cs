using Forge.DataStructures;
using Forge.Factory;

namespace Forge.Serialization.Serializers
{
	public class ForgeSignatureSerializer<T> : ITypeSerializer where T : ISignature
	{
		public object Deserialize(BMSByte buffer)
		{
			byte[] data = ForgeSerializationStrategy.Instance.Deserialize<byte[]>(buffer);
			if (data.Length == 0)
				return null;
			var sig = AbstractFactory.Get<INetworkTypeFactory>().GetNew<T>();
			sig.Deserialize(data);
			return sig;
		}

		public void Serialize(object val, BMSByte buffer)
		{
			var sig = (T)val;
			ForgeSerializationStrategy.Instance.Serialize(sig.Serialize(), buffer);
		}
	}
}

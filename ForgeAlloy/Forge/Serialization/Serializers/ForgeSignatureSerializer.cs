using Forge.DataStructures;
using Forge.Factory;

namespace Forge.Serialization.Serializers
{
	public class ForgeSignatureSerializer<T> : ITypeSerializer where T : ISignature
	{
		public object Deserialize(BMSByte buffer)
		{
			bool hasSig = ForgeSerializer.Instance.Deserialize<bool>(buffer);
			if (!hasSig)
				return null;
			var sig = AbstractFactory.Get<INetworkTypeFactory>().GetNew<T>();
			sig.Deserialize(buffer);
			return sig;
		}

		public void Serialize(object val, BMSByte buffer)
		{
			var sig = (T)val;
			ForgeSerializer.Instance.Serialize(true, buffer);
			sig.Serialize(buffer);
		}
	}
}

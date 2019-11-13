using Forge.DataStructures;

namespace Forge.Serialization.Serializers
{
	public class ForgeSignatureSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			byte[] data = ForgeSerializationStrategy.Instance.Deserialize<byte[]>(buffer);
			if (data.Length == 0)
				return null;
			var sig = new ForgeSignature();
			sig.Deserialize(data);
			return sig;
		}

		public byte[] Serialize(object val)
		{
			var sig = (ForgeSignature)val;
			return ForgeSerializationStrategy.Instance.Serialize(sig.Serialize());
		}
	}
}

using System;

namespace Forge.Serialization.Serializers
{
	public class VectorSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<Vector>();
		}

		public byte[] Serialize(object val)
		{
			var vec = (Vector)val;
			byte[] bytes = new byte[sizeof(float) * 3];
			Buffer.BlockCopy(BitConverter.GetBytes(vec.x), 0, bytes, 0, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(vec.y), 0, bytes, sizeof(float), sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(vec.z), 0, bytes, sizeof(float) * 2, sizeof(float));
			return bytes;
		}
	}
}

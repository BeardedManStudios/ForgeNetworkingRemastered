using System;

namespace Forge.Serialization.Serializers
{
	public class FloatSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<float>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((float)val);
		}
	}
}

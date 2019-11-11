using System;

namespace Forge.Serialization.Serializers
{
	public class LongSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<long>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((long)val);
		}
	}
}

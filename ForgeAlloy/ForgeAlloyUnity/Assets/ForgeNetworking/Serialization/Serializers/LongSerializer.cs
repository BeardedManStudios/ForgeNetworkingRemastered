using System;

namespace Forge.Serialization.Serializers
{
	public class LongSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<long>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((long)val));
		}
	}
}

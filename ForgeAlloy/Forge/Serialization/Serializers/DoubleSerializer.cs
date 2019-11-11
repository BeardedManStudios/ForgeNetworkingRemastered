using System;

namespace Forge.Serialization.Serializers
{
	public class DoubleSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<double>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((double)val);
		}
	}
}

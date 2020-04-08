using System;

namespace Forge.Serialization.Serializers
{
	public class DoubleSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<double>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((double)val));
		}
	}
}

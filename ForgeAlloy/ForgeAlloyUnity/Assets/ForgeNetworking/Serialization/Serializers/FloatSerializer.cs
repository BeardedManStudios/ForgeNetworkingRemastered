using System;

namespace Forge.Serialization.Serializers
{
	public class FloatSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<float>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((float)val));
		}
	}
}

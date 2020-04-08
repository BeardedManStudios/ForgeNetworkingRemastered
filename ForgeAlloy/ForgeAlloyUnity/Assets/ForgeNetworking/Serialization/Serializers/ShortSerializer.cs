using System;

namespace Forge.Serialization.Serializers
{
	public class ShortSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<short>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((short)val));
		}
	}
}

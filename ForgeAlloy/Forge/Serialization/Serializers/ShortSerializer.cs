using System;

namespace Forge.Serialization.Serializers
{
	public class ShortSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<short>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((short)val);
		}
	}
}

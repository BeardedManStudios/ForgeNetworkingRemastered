using System;

namespace Forge.Serialization.Serializers
{
	public class BoolSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<bool>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((bool)val);
		}
	}
}

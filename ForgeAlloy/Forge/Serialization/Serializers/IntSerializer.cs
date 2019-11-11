using System;

namespace Forge.Serialization.Serializers
{
	public class IntSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<int>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((int)val);
		}
	}
}

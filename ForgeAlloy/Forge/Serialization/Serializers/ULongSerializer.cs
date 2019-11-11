using System;

namespace Forge.Serialization.Serializers
{
	public class ULongSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<ulong>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((ulong)val);
		}
	}
}

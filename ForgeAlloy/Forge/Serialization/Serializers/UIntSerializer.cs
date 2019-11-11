using System;

namespace Forge.Serialization.Serializers
{
	public class UIntSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<uint>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((uint)val);
		}
	}
}

using System;

namespace Forge.Serialization.Serializers
{
	public class UShortSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<ushort>();
		}

		public byte[] Serialize(object val)
		{
			return BitConverter.GetBytes((ushort)val);
		}
	}
}

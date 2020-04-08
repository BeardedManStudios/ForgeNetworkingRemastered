using System;

namespace Forge.Serialization.Serializers
{
	public class UShortSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<ushort>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((ushort)val));
		}
	}
}

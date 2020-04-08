using System;

namespace Forge.Serialization.Serializers
{
	public class ULongSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<ulong>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((ulong)val));
		}
	}
}

using System;

namespace Forge.Serialization.Serializers
{
	public class UIntSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<uint>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((uint)val));
		}
	}
}

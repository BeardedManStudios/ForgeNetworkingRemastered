using System;

namespace Forge.Serialization.Serializers
{
	public class BoolSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<bool>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((bool)val));
		}
	}
}

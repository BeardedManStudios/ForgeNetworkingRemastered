using System;

namespace Forge.Serialization.Serializers
{
	public class IntSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<int>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(BitConverter.GetBytes((int)val));
		}
	}
}

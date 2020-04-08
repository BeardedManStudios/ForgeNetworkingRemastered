using System;
using System.Text;

namespace Forge.Serialization.Serializers
{
	public class StringSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<string>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			var strBytes = Encoding.UTF8.GetBytes(val == null ? string.Empty : (string)val);
			buffer.Append(BitConverter.GetBytes(strBytes.Length));
			buffer.Append(strBytes);
		}
	}
}

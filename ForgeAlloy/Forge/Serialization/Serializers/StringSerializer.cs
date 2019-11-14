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

		public byte[] Serialize(object val)
		{
			var strBytes = Encoding.UTF8.GetBytes(val == null ? string.Empty : (string)val);
			byte[] bytes = new byte[strBytes.Length + sizeof(int)];
			Buffer.BlockCopy(BitConverter.GetBytes(strBytes.Length), 0, bytes, 0, sizeof(int));
			if (strBytes.Length > 0)
				Buffer.BlockCopy(strBytes, 0, bytes, sizeof(int), strBytes.Length);
			return bytes;
		}
	}
}

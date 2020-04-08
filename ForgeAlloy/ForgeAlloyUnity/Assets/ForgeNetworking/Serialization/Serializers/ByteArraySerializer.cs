using System;
using System.IO;

namespace Forge.Serialization.Serializers
{
	public class ByteArraySerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			int length = buffer.GetBasicType<int>();
			return buffer.GetByteRange(length);
		}

		public void Serialize(object val, BMSByte buffer)
		{
			byte[] obj = (byte[])val;
			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(BitConverter.GetBytes(obj.Length));
					writer.Write(obj);
				}
				buffer.Append(stream.ToArray());
			}
		}
	}
}

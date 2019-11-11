namespace Forge.Serialization.Serializers
{
	public class ByteSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<byte>();
		}

		public byte[] Serialize(object val)
		{
			return new byte[1] { (byte)val };
		}
	}
}

namespace Forge.Serialization.Serializers
{
	public class ByteSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<byte>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(new byte[1] { (byte)val });
		}
	}
}

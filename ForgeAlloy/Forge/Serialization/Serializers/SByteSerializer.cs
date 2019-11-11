namespace Forge.Serialization.Serializers
{
	public class SByteSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<sbyte>();
		}

		public byte[] Serialize(object val)
		{
			return new byte[1] { (byte)((sbyte)val) };
		}
	}
}

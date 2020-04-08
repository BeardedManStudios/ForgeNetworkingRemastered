namespace Forge.Serialization.Serializers
{
	public class SByteSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<sbyte>();
		}

		public void Serialize(object val, BMSByte buffer)
		{
			buffer.Append(new byte[1] { (byte)((sbyte)val) });
		}
	}
}

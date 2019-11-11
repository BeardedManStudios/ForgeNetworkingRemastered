namespace Forge.Serialization
{
	public interface ITypeSerializer
	{
		byte[] Serialize(object val);
		object Deserialize(BMSByte buffer);
	}
}

namespace Forge.Serialization
{
	public interface ITypeSerializer
	{
		void Serialize(object val, BMSByte buffer);
		object Deserialize(BMSByte buffer);
	}
}

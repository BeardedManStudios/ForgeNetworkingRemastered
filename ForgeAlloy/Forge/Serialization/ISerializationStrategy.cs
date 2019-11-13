namespace Forge.Serialization
{
	public interface ISerializationStrategy
	{
		void AddSerializer<T>(ITypeSerializer serializer);
		byte[] Serialize<T>(T val);
		T Deserialize<T>(BMSByte buffer);
		void Clear();
	}
}

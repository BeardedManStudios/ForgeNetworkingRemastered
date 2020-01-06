namespace Forge.Serialization
{
	public interface ISerializationStrategy
	{
		void AddSerializer<T>(ITypeSerializer serializer);
		void Serialize<T>(T val, BMSByte buffer);
		T Deserialize<T>(BMSByte buffer);
		void Clear();
	}
}

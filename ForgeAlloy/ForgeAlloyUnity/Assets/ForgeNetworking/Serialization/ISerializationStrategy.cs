namespace Forge.Serialization
{
	public interface ISerializationStrategy
	{
		void AddSerializer<T>(ITypeSerializer serializer);
		void Serialize<T>(T val, BMSByte intoBuffer);
		T Deserialize<T>(BMSByte buffer);
		void Clear();
	}
}

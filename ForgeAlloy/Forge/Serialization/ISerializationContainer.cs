namespace Forge.Serialization
{
	public interface ISerializationContainer
	{
		void AddSerializer<T>(ITypeSerializer serializer);
		byte[] Serialize<T>(T val);
		T Deserialize<T>(BMSByte buffer);
		void Clear();
	}
}

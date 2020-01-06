using Forge.Serialization;

namespace ForgeTests.Serialization.Serializers
{
	public class TypeSerializerTestsBase
	{
		protected T SerializeDeserialize<T>(ITypeSerializer serializer, T value)
		{
			var buffer = new BMSByte();
			serializer.Serialize(value, buffer);
			buffer.MoveStartIndex(-buffer.StartIndex());
			return (T)serializer.Deserialize(buffer);
		}
	}
}

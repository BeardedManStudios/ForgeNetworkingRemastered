using Forge.Serialization;

namespace ForgeTests.Serialization.Serializers
{
	public class TypeSerializerTestsBase
	{
		protected T SerializeDeserialize<T>(ITypeSerializer serializer, T value)
		{
			var buffer = new BMSByte();
			buffer.Append(serializer.Serialize(value));
			buffer.MoveStartIndex(-buffer.StartIndex());
			return (T)serializer.Deserialize(buffer);
		}
	}
}

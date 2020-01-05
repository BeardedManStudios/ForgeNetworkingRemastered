using Forge.Serialization;

namespace Forge.Networking.Unity.Tests.Serializers
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

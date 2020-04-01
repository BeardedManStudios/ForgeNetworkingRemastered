using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class BoolSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingFalseBool_ShouldBeEqual()
		{
			bool input = false;
			Assert.AreEqual(input, SerializeDeserialize(new BoolSerializer(), input));
		}

		[Test]
		public void SerializingTrueBool_ShouldBeEqual()
		{
			bool input = true;
			Assert.AreEqual(input, SerializeDeserialize(new BoolSerializer(), input));
		}
	}
}

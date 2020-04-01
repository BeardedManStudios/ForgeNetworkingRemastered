using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class StringSerializationTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingString_ShouldBeEqual()
		{
			string input = Guid.NewGuid().ToString();
			Assert.AreEqual(input, SerializeDeserialize(new StringSerializer(), input));
		}
	}
}

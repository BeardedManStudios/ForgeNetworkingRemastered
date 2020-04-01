using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class FloatSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingFloat_ShouldBeEqual()
		{
			float input = (float)new Random().NextDouble();
			Assert.AreEqual(input, SerializeDeserialize(new FloatSerializer(), input));
		}
	}
}

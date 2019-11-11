using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class DoubleSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingDouble_ShouldBeEqual()
		{
			double input = new Random().NextDouble();
			Assert.AreEqual(input, SerializeDeserialize(new DoubleSerializer(), input));
		}
	}
}

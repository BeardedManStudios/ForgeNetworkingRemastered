using System;
using Forge.Serialization;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class VectorSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingVector_ShouldBeEqual()
		{
			var rand = new Random();
			var input = new Vector(
				(float)rand.NextDouble(),
				(float)rand.NextDouble(),
				(float)rand.NextDouble()
			);
			Assert.AreEqual(input, SerializeDeserialize(new VectorSerializer(), input));
		}
	}
}

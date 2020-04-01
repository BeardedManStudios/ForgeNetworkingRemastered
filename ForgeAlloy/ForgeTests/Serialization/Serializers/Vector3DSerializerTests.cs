using System;
using Forge.DataStructures;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class Vector3DSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingVector_ShouldBeEqual()
		{
			var rand = new Random();
			var input = new Vector3D(
				(float)rand.NextDouble(),
				(float)rand.NextDouble(),
				(float)rand.NextDouble()
			);
			Assert.AreEqual(input, SerializeDeserialize(new Vector3DSerializer(), input));
		}
	}
}

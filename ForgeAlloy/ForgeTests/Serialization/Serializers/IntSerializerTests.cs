using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class IntSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingInt_ShouldBeEqual()
		{
			int input = new Random().Next(int.MinValue, int.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new IntSerializer(), input));
		}
	}
}

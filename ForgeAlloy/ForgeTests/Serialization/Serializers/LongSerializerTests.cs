using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class LongSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingLong_ShouldBeEqual()
		{
			long input = new Random().Next(int.MinValue, int.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new LongSerializer(), input));
		}
	}
}

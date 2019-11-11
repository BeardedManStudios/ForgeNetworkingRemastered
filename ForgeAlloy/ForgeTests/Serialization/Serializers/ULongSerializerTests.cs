using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class ULongSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingULong_ShouldBeEqual()
		{
			ulong input = (ulong)new Random().Next(0, int.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new ULongSerializer(), input));
		}
	}
}

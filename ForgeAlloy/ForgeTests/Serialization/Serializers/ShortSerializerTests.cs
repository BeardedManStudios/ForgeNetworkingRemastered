using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class ShortSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingShort_ShouldBeEqual()
		{
			short input = (short)new Random().Next(short.MinValue, short.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new ShortSerializer(), input));
		}
	}
}

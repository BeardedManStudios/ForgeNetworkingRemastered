using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class UShortSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingUShort_ShouldBeEqual()
		{
			ushort input = (ushort)new Random().Next(ushort.MinValue, ushort.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new UShortSerializer(), input));
		}
	}
}

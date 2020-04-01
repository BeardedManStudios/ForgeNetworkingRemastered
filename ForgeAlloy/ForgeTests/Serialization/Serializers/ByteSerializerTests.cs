using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class ByteSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingByte_ShouldBeEqual()
		{
			byte input = (byte)new Random().Next(byte.MinValue, byte.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new ByteSerializer(), input));
		}
	}
}

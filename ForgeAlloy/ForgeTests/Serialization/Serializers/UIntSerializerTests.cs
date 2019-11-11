using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class UIntSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingUInt_ShouldBeEqual()
		{
			uint input = (uint)new Random().Next(0, int.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new UIntSerializer(), input));
		}
	}
}

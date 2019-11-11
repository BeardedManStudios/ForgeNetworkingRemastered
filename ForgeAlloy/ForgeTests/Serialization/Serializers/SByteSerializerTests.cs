using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class SByteSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingSByte_ShouldBeEqual()
		{
			sbyte input = (sbyte)new Random().Next(sbyte.MinValue, sbyte.MaxValue);
			Assert.AreEqual(input, SerializeDeserialize(new SByteSerializer(), input));
		}
	}
}

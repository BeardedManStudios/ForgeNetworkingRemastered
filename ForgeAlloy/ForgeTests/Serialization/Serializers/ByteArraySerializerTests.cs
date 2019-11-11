using System;
using Forge.Serialization.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class ByteArraySerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingByteArray_ShouldBeEqual()
		{
			byte[] input = new byte[128];
			new Random().NextBytes(input);
			var res = SerializeDeserialize(new ByteArraySerializer(), input);
			Assert.AreEqual(input.Length, res.Length);
			for (int i = 0; i < input.Length; i++)
				Assert.AreEqual(input[i], res[i]);
		}
	}
}

using Forge.Networking.Unity.Serialization;
using NUnit.Framework;
using UnityEngine;

namespace Forge.Networking.Unity.Tests.Serializers
{
	[TestFixture]
	public class QuaternionSerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void serialize_and_deserialize_should_be_equal()
		{
			var rand = new System.Random();
			var input = new Quaternion(
				(float)rand.NextDouble(),
				(float)rand.NextDouble(),
				(float)rand.NextDouble(),
				(float)rand.NextDouble()
			);
			Assert.AreEqual(input, SerializeDeserialize(new QuaternionSerializer(), input));
		}
	}
}

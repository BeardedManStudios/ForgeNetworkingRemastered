using Forge.DataStructures;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.DataStructures
{
	[TestFixture]
	public class SignatureTests : ForgeNetworkingTest
	{
		private void AssertSerialization<T>() where T : ISignature
		{
			var sig = AbstractFactory.Get<INetworkTypeFactory>().GetNew<T>();
			BMSByte buffer = new BMSByte();
			sig.Serialize(buffer);
			buffer.ResetPointer();
			var sigRes = AbstractFactory.Get<INetworkTypeFactory>().GetNew<T>();
			sigRes.Deserialize(buffer);
			Assert.AreEqual(sig, sigRes);
		}

		[Test]
		public void SignatureSerialization_ShouldMatch()
		{
			AssertSerialization<ISignature>();
		}

		[Test]
		public void PlayerSignatureSerialization_ShouldMatch()
		{
			AssertSerialization<IPlayerSignature>();
		}

		[Test]
		public void MessageReceiptSignatureSerialization_ShouldMatch()
		{
			AssertSerialization<IMessageReceiptSignature>();
		}
	}
}

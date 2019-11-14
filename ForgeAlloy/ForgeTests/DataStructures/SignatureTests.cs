using Forge.DataStructures;
using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using NUnit.Framework;

namespace ForgeTests.DataStructures
{
	[TestFixture]
	public class SignatureTests : ForgeNetworkingTest
	{
		private void AssertSerialization<T>() where T : ISignature
		{
			var sig = AbstractFactory.Get<INetworkTypeFactory>().GetNew<T>();
			byte[] data = sig.Serialize();
			var sigRes = AbstractFactory.Get<INetworkTypeFactory>().GetNew<T>();
			sigRes.Deserialize(data);
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

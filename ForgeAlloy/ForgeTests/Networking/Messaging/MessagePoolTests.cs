using System;
using Forge.Networking.Messaging;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging
{
	[TestFixture]
	public class MessagePoolTests
	{
		private class DummyMessage : IMessage
		{
			public IMessageReceiptSignature Receipt { get; set; }
			public IMessageInterpreter Interpreter => throw new NotImplementedException();

			public void Deserialize(BMSByte buffer)
			{
				throw new NotImplementedException();
			}

			public void Serialize(BMSByte buffer)
			{
				throw new NotImplementedException();
			}
		}

		private class AltDummyMessage : IMessage
		{
			public IMessageReceiptSignature Receipt { get; set; }
			public IMessageInterpreter Interpreter => throw new NotImplementedException();

			public void Deserialize(BMSByte buffer)
			{
				throw new NotImplementedException();
			}

			public void Serialize(BMSByte buffer)
			{
				throw new NotImplementedException();
			}
		}

		[Test]
		public void Get_should_produce_correct_message_type()
		{
			var pool = new MessagePool();
			IMessage d = pool.Get<DummyMessage>();
			Assert.AreEqual(typeof(DummyMessage), d.GetType());
		}

		[Test]
		public void Get_should_produce_same_message()
		{
			var pool = new MessagePool();
			IMessage d = pool.Get<DummyMessage>();
			pool.Release(d);
			IMessage q = pool.Get<DummyMessage>();
			Assert.AreEqual(d, q);
		}

		[Test]
		public void Get_should_produce_different_messages()
		{
			var pool = new MessagePool();
			IMessage d = pool.Get<DummyMessage>();
			IMessage q = pool.Get<DummyMessage>();
			Assert.AreNotEqual(d, q);
		}

		[Test]
		public void Get_should_produce_different_message_types()
		{
			var pool = new MessagePool();
			IMessage d = pool.Get<DummyMessage>();
			IMessage a = pool.Get<AltDummyMessage>();
			Assert.AreNotEqual(d.GetType(), a.GetType());
		}
	}
}

using System;
using Forge.Networking.Messaging;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging
{
	[TestFixture]
	public class MessagePoolMultiTests
	{
		private class DummyMessage : ForgeMessage
		{
			public override IMessageInterpreter Interpreter => throw new NotImplementedException();
			public override void Deserialize(BMSByte buffer) { }
			public override void Serialize(BMSByte buffer) { }
		}

		private class AltDummyMessage : ForgeMessage
		{
			public override IMessageInterpreter Interpreter => throw new NotImplementedException();
			public override void Deserialize(BMSByte buffer) { }
			public override void Serialize(BMSByte buffer) { }
		}

		[Test]
		public void Get_generic_should_produce_correct_message_type()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get<DummyMessage>();
			Assert.AreEqual(typeof(DummyMessage), d.GetType());
		}

		[Test]
		public void Get_should_produce_correct_message_type()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get(typeof(DummyMessage));
			Assert.AreEqual(typeof(DummyMessage), d.GetType());
		}

		[Test]
		public void Get_generic_should_produce_same_message()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get<DummyMessage>();
			d.Sent();
			IMessage q = pool.Get<DummyMessage>();
			Assert.AreEqual(d, q);
		}

		[Test]
		public void Get_should_produce_same_message()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get(typeof(DummyMessage));
			d.Sent();
			IMessage q = pool.Get(typeof(DummyMessage));
			Assert.AreEqual(d, q);
		}

		[Test]
		public void Get_mixed_should_produce_same_message()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get<DummyMessage>();
			d.Sent();
			IMessage q = pool.Get(typeof(DummyMessage));
			Assert.AreEqual(d, q);
		}

		[Test]
		public void Get_mixed_inverse_should_produce_same_message()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get(typeof(DummyMessage));
			d.Sent();
			IMessage q = pool.Get<DummyMessage>();
			Assert.AreEqual(d, q);
		}

		[Test]
		public void Get_generic_should_produce_different_messages()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get<DummyMessage>();
			IMessage q = pool.Get<DummyMessage>();
			Assert.AreNotEqual(d, q);
		}

		[Test]
		public void Get_should_produce_different_messages()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get(typeof(DummyMessage));
			IMessage q = pool.Get(typeof(DummyMessage));
			Assert.AreNotEqual(d, q);
		}

		[Test]
		public void Get_mixed_should_produce_different_messages()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get<DummyMessage>();
			IMessage q = pool.Get(typeof(DummyMessage));
			Assert.AreNotEqual(d, q);
		}

		[Test]
		public void Get_mixed_inverse_should_produce_different_messages()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get(typeof(DummyMessage));
			IMessage q = pool.Get<DummyMessage>();
			Assert.AreNotEqual(d, q);
		}

		[Test]
		public void Get_generic_should_produce_different_message_types()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get<DummyMessage>();
			IMessage a = pool.Get<AltDummyMessage>();
			Assert.AreNotEqual(d.GetType(), a.GetType());
		}

		[Test]
		public void Get_should_produce_different_message_types()
		{
			var pool = new MessagePoolMulti();
			IMessage d = pool.Get(typeof(DummyMessage));
			IMessage a = pool.Get(typeof(AltDummyMessage));
			Assert.AreNotEqual(d.GetType(), a.GetType());
		}
	}
}

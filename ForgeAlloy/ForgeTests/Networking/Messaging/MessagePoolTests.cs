using System;
using System.Collections.Generic;
using Forge.Networking.Messaging;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging
{
	[TestFixture]
	public class MessagePoolTests
	{
		private class DummyMessage : ForgeMessage
		{
			public override IMessageInterpreter Interpreter => throw new NotImplementedException();
			public override void Deserialize(BMSByte buffer) { }
			public override void Serialize(BMSByte buffer) { }
		}

		[Test]
		public void Get_should_produce_new_message()
		{
			var pool = new MessagePool<DummyMessage>();
			IMessage d = pool.Get();
			Assert.NotNull(d);
		}

		[Test]
		public void Get_should_produce_same_message()
		{
			var pool = new MessagePool<DummyMessage>();
			IMessage d = pool.Get();
			d.Sent();
			IMessage q = pool.Get();
			Assert.AreEqual(d, q);
		}

		[Test]
		public void Get_should_produce_different_messages()
		{
			var pool = new MessagePool<DummyMessage>();
			var current = new List<IMessage>();
			current.Add(pool.Get());
			for (int i = 0; i < 5; i++)
			{
				IMessage m = pool.Get();
				for (int j = 0; j < current.Count; j++)
					Assert.AreNotEqual(current[j], m);
				current.Add(m);
			}
		}
	}
}

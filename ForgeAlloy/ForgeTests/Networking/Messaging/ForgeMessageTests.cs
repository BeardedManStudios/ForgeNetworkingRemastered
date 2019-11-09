using FakeItEasy;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Serialization;
using NUnit.Framework;
using System;

namespace Forge.Tests.Networking.Messaging
{
	[TestFixture]
	public class ForgeMessageTests
	{
		private static IMessage interpretedMessage = null;

		public class ForgeMessageMock : ForgeMessage
		{
			public string MockString { get; set; } = "";
			public bool ItWorked { get; set; } = false;

			public override IMessageInterpreter Interpreter
			{
				get
				{
					var mockInterpreter = A.Fake<IMessageInterpreter>();
					A.CallTo(() => mockInterpreter.Interpret(A<INetwork>._, A<IMessage>._)).Invokes((ctx) =>
					{
						interpretedMessage = (ForgeMessage)ctx.Arguments[1];
						ItWorked = true;
					});
					return mockInterpreter;
				}
			}

			public override void Deserialize(BMSByte buffer)
			{
				MockString = buffer.GetBasicType<string>();
			}

			public override void Serialize(BMSByte buffer)
			{
				ObjectMapper.Instance.MapBytes(buffer, MockString);
			}
		}

		[SetUp]
		public void Setup()
		{
			ForgeMessageCodes.Register<ForgeMessageMock>(ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE);
		}

		[TearDown]
		public void Teardown()
		{
			ForgeMessageCodes.Unregister(ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE);
			interpretedMessage = null;
		}

		[Test]
		public void MessageSerialization_ShouldMatch()
		{
			var host = A.Fake<INetwork>();
			var receipt = new ForgeMessageReceipt();
			receipt.Signature = Guid.NewGuid();

			var mock = new ForgeMessageMock();
			mock.MessageCode = ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE;
			mock.MockString = "This is a test message";
			mock.Receipt = receipt;

			byte[] bin = null;
			var rec = A.Fake<IMessageClient>();
			A.CallTo(() => rec.Send(A<byte[]>._)).Invokes((ctx) =>
			{
				bin = (byte[])ctx.Arguments[0];
			});
			var bus = new ForgeMessageBus();
			bus.SendMessage(mock, rec);
			Assert.IsNotNull(bin);

			var sender = A.Fake<IMessageClient>();
			bus.ReceiveMessageBuffer(host, sender, bin);

			Assert.AreEqual(mock.MessageCode, interpretedMessage.MessageCode);
			Assert.AreEqual(mock.Receipt.Signature, interpretedMessage.Receipt.Signature);

			var res = (ForgeMessageMock)interpretedMessage;
			Assert.IsTrue(res.ItWorked);
			Assert.AreEqual(mock.MockString, res.MockString);
			Assert.AreNotEqual(mock.ItWorked, res.ItWorked);
		}
	}
}

using System;
using FakeItEasy;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging
{
	[TestFixture]
	public class ForgeMessageTests : ForgeNetworkingTest
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
					A.CallTo(() => mockInterpreter.Interpret(A<INetworkContainer>._, A<IMessage>._)).Invokes((ctx) =>
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

		public override void Setup()
		{
			base.Setup();
			ForgeMessageCodes.Register<ForgeMessageMock>(ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE);
		}

		public override void Teardown()
		{
			base.Teardown();
			ForgeMessageCodes.Unregister(ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE);
			interpretedMessage = null;
		}

		[Test]
		public void MessageSerialization_ShouldMatch()
		{
			var host = A.Fake<INetworkContainer>();
			var receipt = new ForgeMessageReceipt();
			receipt.Signature = Guid.NewGuid();

			var mock = new ForgeMessageMock();
			mock.MessageCode = ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE;
			mock.MockString = "This is a test message";
			mock.Receipt = receipt;

			byte[] bin = null;
			var rec = A.Fake<ISocket>();
			A.CallTo(() => rec.Send(A<byte[]>._, A<int>._)).Invokes((ctx) =>
			{
				bin = (byte[])ctx.Arguments[0];
			});
			var bus = new ForgeMessageBus();
			bus.SendMessage(mock, rec);
			Assert.IsNotNull(bin);

			var sender = A.Fake<ISocket>();
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

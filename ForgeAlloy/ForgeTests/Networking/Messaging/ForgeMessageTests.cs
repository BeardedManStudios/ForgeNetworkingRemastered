using System;
using FakeItEasy;
using Forge.Networking;
using Forge.Networking.Messaging;
using NUnit.Framework;

namespace Forge.Tests.Networking.Messaging
{
	public class MessageInterpreterMock : IMessageInterpreter
	{
		public void Interpret(INetworkHost netHost, IMessage message)
		{
			var m = (ForgeMessageMock)message;
			m.ItWorked = true;
		}
	}

	public class ForgeMessageMock : ForgeMessage
	{
		public string MockString { get; set; } = "";
		public bool ItWorked { get; set; } = false;

		public override IMessageInterpreter Interpreter => new MessageInterpreterMock();

		public override void DeserializeData(BMSByte buffer)
		{
			MockString = buffer.GetBasicType<string>();
		}

		public override void SerializeData(BMSByte buffer)
		{
			ObjectMapper.Instance.MapBytes(buffer, MockString);
		}
	}

	[TestFixture]
	public class ForgeMessageTests
	{
		[SetUp]
		public void Setup()
		{
			ForgeMessageCodes.Register<ForgeMessageMock>(ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE);
		}

		[TearDown]
		public void Teardown()
		{
			ForgeMessageCodes.Unregister(ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE);
		}

		[Test]
		public void MessageSerialization_ShouldMatch()
		{
			var host = A.Fake<INetworkHost>();
			var receipt = new ForgeMessageReceipt();
			receipt.Signature = Guid.NewGuid();
			var mock = new ForgeMessageMock
			{
				MessageCode = ForgeMessageCodes.UNIT_TEST_MOCK_MESSAGE,
				MockString = "This is a test message",
				Receipt = receipt
			};

			byte[] bin = mock.Serialize();
			var target = ForgeMessage.Deserialize(bin);
			target.Interpret(host);
			Assert.AreEqual(mock.MessageCode, target.MessageCode);
			Assert.AreEqual(mock.Receipt.Signature, target.Receipt.Signature);

			var mockTarget = (ForgeMessageMock)target;
			Assert.IsTrue(mockTarget.ItWorked);
			Assert.AreEqual(mock.MockString, mockTarget.MockString);
			Assert.AreNotEqual(mock.ItWorked, mockTarget.ItWorked);
		}
	}
}

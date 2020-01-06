using System.Net;
using System.Threading;
using FakeItEasy;
using Forge.Factory;
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

		[UnitTestingMessageContract(1, typeof(ForgeMessageMock))]
		public class ForgeMessageMock : ForgeMessage
		{
			public string MockString { get; set; } = "";
			public bool ItWorked { get; set; } = false;

			public override IMessageInterpreter Interpreter
			{
				get
				{
					var mockInterpreter = A.Fake<IMessageInterpreter>();
					A.CallTo(() => mockInterpreter.ValidOnClient).Returns(true);
					A.CallTo(() => mockInterpreter.ValidOnServer).Returns(true);
					A.CallTo(() => mockInterpreter.Interpret(A<INetworkMediator>._, A<EndPoint>._, A<IMessage>._)).Invokes((ctx) =>
					{
						interpretedMessage = (ForgeMessage)ctx.Arguments[2];
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
				ForgeSerializer.Instance.Serialize(MockString, buffer);
			}
		}

		public override void Setup()
		{
			base.Setup();
			SynchronizationContext.SetSynchronizationContext(new ForgeTestSynchronizationContext());
		}

		public override void Teardown()
		{
			base.Teardown();
			interpretedMessage = null;
		}

		[Test]
		public void MessageSerialization_ShouldMatch()
		{
			var mediator = A.Fake<INetworkMediator>();
			var receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();
			receipt = null;

			var mock = new ForgeMessageMock();
			mock.MockString = "This is a test message";
			mock.Receipt = receipt;

			BMSByte bin = null;
			var receiver = A.Fake<ISocket>();
			var sender = A.Fake<ISocket>();
			A.CallTo(() => sender.Send(A<EndPoint>._, A<BMSByte>._)).Invokes((ctx) =>
			{
				var b = (BMSByte)ctx.Arguments[1];
				bin = new BMSByte();
				bin.Clone(b);
			});
			var bus = new ForgeMessageBus();
			bus.SetMediator(mediator);
			bus.SendMessage(mock, sender, receiver.EndPoint);
			Assert.IsNotNull(bin);

			bus.ReceiveMessageBuffer(receiver, sender.EndPoint, bin);

			Assert.IsNull(interpretedMessage.Receipt);

			var res = (ForgeMessageMock)interpretedMessage;
			Assert.IsTrue(res.ItWorked);
			Assert.AreEqual(mock.MockString, res.MockString);
			Assert.AreNotEqual(mock.ItWorked, res.ItWorked);
		}

		[Test]
		public void ReliableMessageSerialization_ShouldMatch()
		{
			var mediator = A.Fake<INetworkMediator>();
			A.CallTo(() => mediator.PlayerTimeout).Returns(1);
			var tokenSource = new CancellationTokenSource();
			A.CallTo(() => mediator.SocketFacade.CancellationSource).Returns(tokenSource);
			var receipt = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageReceiptSignature>();

			var mock = new ForgeMessageMock();
			mock.MockString = "This is a test message";
			mock.Receipt = receipt;

			BMSByte bin = null;
			var receiver = A.Fake<ISocket>();
			var sender = A.Fake<ISocket>();
			A.CallTo(() => sender.Send(A<EndPoint>._, A<BMSByte>._)).Invokes((ctx) =>
			{
				var b = (BMSByte)ctx.Arguments[1];
				bin = new BMSByte();
				bin.Clone(b);
			});
			var bus = new ForgeMessageBus();
			bus.SetMediator(mediator);
			bus.SendReliableMessage(mock, sender, receiver.EndPoint);
			tokenSource.Cancel();
			Assert.IsNotNull(bin);

			bus.ReceiveMessageBuffer(receiver, sender.EndPoint, bin);

			Assert.IsNotNull(interpretedMessage.Receipt);
			Assert.AreEqual(mock.Receipt, interpretedMessage.Receipt);

			var res = (ForgeMessageMock)interpretedMessage;
			Assert.IsTrue(res.ItWorked);
			Assert.AreEqual(mock.MockString, res.MockString);
			Assert.AreNotEqual(mock.ItWorked, res.ItWorked);
		}
	}
}

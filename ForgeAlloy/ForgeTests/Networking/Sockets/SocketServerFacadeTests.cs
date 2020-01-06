using System.Net;
using System.Threading;
using FakeItEasy;
using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Interpreters;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Socket
{
	[TestFixture]
	public class SocketServerFacadeTests : ForgeNetworkingTest
	{
		private SynchronizationContext _syncCtx;

		public override void Setup()
		{
			base.Setup();
			_syncCtx = new ForgeTestSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(_syncCtx);
		}

		[Test]
		public void ConnectingPlayer_ShouldBePresentedAChallenge()
		{
			var netContainer = A.Fake<INetworkMediator>();
			var serverFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			serverFacade.StartServer(15937, 10, netContainer);
			var client = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IClientSocket>();
			client.Connect(CommonSocketBase.LOCAL_IPV4, 15937);
			BMSByte buffer = new BMSByte();
			buffer.Append(new byte[] { 1 });
			client.Send(client.EndPoint, buffer);
			Thread.Sleep(50);
			A.CallTo(() => netContainer.MessageBus.SendReliableMessage(A<IChallengeMessage>._,
				A<ISocket>._, A<EndPoint>._)).MustHaveHappenedOnceExactly();
			client.Close();
			serverFacade.ShutDown();
		}

		[Test]
		public void ConnectingPlayer_ShouldRespondToAChallenge()
		{
			var netContainerServer = A.Fake<INetworkMediator>();
			var netContainerClient = A.Fake<INetworkMediator>();
			var serverFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			serverFacade.StartServer(15937, 10, netContainerServer);
			var clientFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketClientFacade>();
			bool done = false;
			A.CallTo(() => netContainerServer.MessageBus.SendReliableMessage(A<IChallengeMessage>._,
				A<ISocket>._, A<EndPoint>._)).Invokes((ctx) =>
				{
					var interpreter = new ForgeConnectChallengeInterpreter();
					interpreter.Interpret(netContainerClient, A.Fake<EndPoint>(), (IChallengeMessage)ctx.Arguments[0]);
					A.CallTo(() => netContainerClient.MessageBus.SendReliableMessage(A<IChallengeResponseMessage>._,
						A<ISocket>._, A<EndPoint>._)).MustHaveHappenedOnceExactly();
					done = true;
				});
			var client = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IClientSocket>();
			clientFacade.StartClient(CommonSocketBase.LOCAL_IPV4, 15937, netContainerClient);
			Thread.Sleep(50);
			Assert.IsTrue(done);
			client.Close();
			serverFacade.ShutDown();
		}

		[Test]
		public void ConnectingPlayer_ShouldRespondToAChallengeAndBeCorrect()
		{
			var serverMediator = A.Fake<INetworkMediator>();
			var serverFake = A.Fake<ISocketServerFacade>();
			var clientMediator = A.Fake<INetworkMediator>();
			var serverFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			serverFacade.StartServer(15937, 10, serverMediator);
			var clientFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketClientFacade>();
			A.CallTo(() => serverMediator.SocketFacade).Returns(serverFake);
			A.CallTo(() => clientMediator.SocketFacade).Returns(clientFacade);
			bool done = false;
			A.CallTo(() => clientMediator.MessageBus.SendReliableMessage(A<IChallengeResponseMessage>._,
				A<ISocket>._, A<EndPoint>._)).Invokes((ctx) =>
				{
					Assert.IsTrue(((IChallengeResponseMessage)ctx.Arguments[0]).ValidateResponse());
					var finalInterpreter = new ForgeConnectChallengeResponseInterpreter();
					finalInterpreter.Interpret(serverMediator, A.Fake<EndPoint>(), (IMessage)ctx.Arguments[0]);
					A.CallTo(() => serverFake.ChallengeSuccess(A<INetworkMediator>._, A<EndPoint>._)).MustHaveHappenedOnceExactly();
					done = true;
				});
			A.CallTo(() => serverMediator.MessageBus.SendReliableMessage(A<IChallengeMessage>._,
				A<ISocket>._, A<EndPoint>._)).Invokes((ctx) =>
				{
					var interpreter = new ForgeConnectChallengeInterpreter();
					interpreter.Interpret(clientMediator, A.Fake<EndPoint>(), (IChallengeMessage)ctx.Arguments[0]);
				});
			clientFacade.StartClient(CommonSocketBase.LOCAL_IPV4, 15937, clientMediator);
			Thread.Sleep(50);
			Assert.IsTrue(done);
			serverFacade.ShutDown();
			clientFacade.ShutDown();
		}
	}
}

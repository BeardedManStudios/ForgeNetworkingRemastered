using System.Net;
using System.Threading;
using FakeItEasy;
using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Messaging.Interpreters;
using Forge.Networking.Messaging.Messages;
using Forge.Networking.Sockets;
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
			client.Connect("127.0.0.1", 15937);
			client.Send(client.EndPoint, new byte[] { 1 }, 1);
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
			clientFacade.StartClient("127.0.0.1", 15937, netContainerClient);
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
			client.Connect("127.0.0.1", 15937);
			client.Send(client.EndPoint, new byte[] { 1 }, 1);
			Thread.Sleep(50);
			Assert.IsTrue(done);
			client.Close();
			serverFacade.ShutDown();
		}

		[Test]
		public void ConnectingPlayer_ShouldRespondToAChallengeAndBeCorrect()
		{
			var netContainerServer = A.Fake<INetworkMediator>();
			var serverFake = A.Fake<ISocketServerFacade>();
			var netContainerClient = A.Fake<INetworkMediator>();
			var serverFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketServerFacade>();
			serverFacade.StartServer(15937, 10, netContainerServer);
			var clientFacade = AbstractFactory.Get<INetworkTypeFactory>().GetNew<ISocketClientFacade>();
			clientFacade.StartClient("127.0.0.1", 15937, netContainerClient);
			A.CallTo(() => netContainerServer.SocketFacade).Returns(serverFake);
			A.CallTo(() => netContainerClient.SocketFacade).Returns(clientFacade);
			bool done = false;
			A.CallTo(() => netContainerClient.MessageBus.SendReliableMessage(A<IChallengeResponseMessage>._,
				A<ISocket>._, A<EndPoint>._)).Invokes((ctx) =>
				{
					Assert.IsTrue(((IChallengeResponseMessage)ctx.Arguments[0]).ValidateResponse());
					var finalInterpreter = new ForgeConnectChallengeResponseInterpreter();
					finalInterpreter.Interpret(netContainerServer, A.Fake<EndPoint>(), (IMessage)ctx.Arguments[0]);
					A.CallTo(() => serverFake.ChallengeSuccess(A<INetworkMediator>._, A<EndPoint>._)).MustHaveHappenedOnceExactly();
					done = true;
				});
			A.CallTo(() => netContainerServer.MessageBus.SendReliableMessage(A<IChallengeMessage>._,
				A<ISocket>._, A<EndPoint>._)).Invokes((ctx) =>
				{
					var interpreter = new ForgeConnectChallengeInterpreter();
					interpreter.Interpret(netContainerClient, A.Fake<EndPoint>(), (IChallengeMessage)ctx.Arguments[0]);
				});
			var client = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IClientSocket>();
			client.Connect("127.0.0.1", 15937);
			client.Send(client.EndPoint, new byte[] { 1 }, 1);
			Thread.Sleep(50);
			Assert.IsTrue(done);
			client.Close();
			serverFacade.ShutDown();
		}
	}
}

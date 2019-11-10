using System.Net;
using System.Threading;
using FakeItEasy;
using Forge;
using Forge.Networking;
using Forge.Networking.Players;
using Forge.Networking.Sockets;
using NUnit.Framework;

namespace ForgeTests.Networking.Socket
{
	[TestFixture]
	public class SocketServerContainerTests : ForgeNetworkingTest
	{
		public override void Setup()
		{
			base.Setup();
			SynchronizationContext.SetSynchronizationContext(new ForgeTestSynchronizationContext());
		}

		[Test]
		public void ConnectingPlayer_ShouldBeAddedToRepositoryAndReportedToEngine()
		{
			var netContainer = A.Fake<INetworkContainer>();
			var serverContainer = ForgeTypeFactory.GetNew<ISocketServerContainer>();
			serverContainer.StartServer("127.0.0.1", 15937, 10, netContainer);
			var client = ForgeTypeFactory.GetNew<IClientSocket>();
			client.Connect("127.0.0.1", 15937);
			var serverAcceptSocketHandle = A.Fake<ISocket>();
			A.CallTo(() => serverAcceptSocketHandle.EndPoint).Returns(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15937));
			client.Send(serverAcceptSocketHandle, new byte[] { 1 }, 1);
			Thread.Sleep(90);
			A.CallTo(() => netContainer.PlayerRepository.AddPlayer(A<INetPlayer>._)).MustHaveHappenedOnceExactly();
			A.CallTo(() => netContainer.EngineContainer.PlayerJoined(A<INetPlayer>._)).MustHaveHappenedOnceExactly();
			client.Close();
			serverContainer.ShutDown();
		}
	}
}

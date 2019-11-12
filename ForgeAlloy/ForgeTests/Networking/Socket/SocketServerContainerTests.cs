using System.Threading;
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

		// TODO:  Connect flow causes this test to be invalid and needs to be re-created
		//[Test]
		//public void ConnectingPlayer_ShouldBeAddedToRepositoryAndReportedToEngine()
		//{
		//	var netContainer = A.Fake<INetworkContainer>();
		//	var serverContainer = ForgeTypeFactory.GetNew<ISocketServerContainer>();
		//	serverContainer.StartServer(15937, 10, netContainer);
		//	var client = ForgeTypeFactory.GetNew<IClientSocket>();
		//	client.Connect("127.0.0.1", 15937);
		//	var serverAcceptSocketHandle = A.Fake<ISocket>();
		//	A.CallTo(() => serverAcceptSocketHandle.EndPoint).Returns(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15937));
		//	client.Send(serverAcceptSocketHandle.EndPoint, new byte[] { 1 }, 1);
		//	Thread.Sleep(50);
		//	A.CallTo(() => netContainer.PlayerRepository.AddPlayer(A<INetPlayer>._)).MustHaveHappenedOnceExactly();
		//	client.Close();
		//	serverContainer.ShutDown();
		//}
	}
}

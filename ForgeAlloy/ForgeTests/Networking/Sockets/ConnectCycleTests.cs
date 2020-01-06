using System.Threading;
using FakeItEasy;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Sockets;
using NUnit.Framework;

namespace ForgeTests.Networking.Sockets
{
	[TestFixture]
	public class ConnectCycleTests : ForgeNetworkingTest
	{

		public override void Setup()
		{
			base.Setup();
			SynchronizationContext.SetSynchronizationContext(new ForgeTestSynchronizationContext());
		}

		[Test]
		public void ConnectCycle()
		{
			var serverEngine = A.Fake<IEngineProxy>();
			var clientEngine = A.Fake<IEngineProxy>();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			var serverMediator = factory.GetNew<INetworkMediator>();
			serverMediator.ChangeEngineProxy(serverEngine);
			var clientMediator = factory.GetNew<INetworkMediator>();
			clientMediator.ChangeEngineProxy(clientEngine);
			serverMediator.StartServer(15937, 64);
			clientMediator.StartClient(CommonSocketBase.LOCAL_IPV4, 15937);
			bool done = false;
			A.CallTo(() => serverEngine.NetworkingEstablished()).MustHaveHappenedOnceExactly();
			A.CallTo(() => clientEngine.NetworkingEstablished()).Invokes((ctx) =>
			{
				done = true;
			});
			Thread.Sleep(500);
			Assert.IsTrue(done);
			clientMediator.SocketFacade.ShutDown();
		}
	}
}

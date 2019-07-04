using System.Net;
using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.TCP
{
	[TestFixture]
	public class TCPServerTests : BaseTest
	{
		private TCPServer server = null;

		[TearDown]
		public void DisposeServer()
		{
			server.Disconnect(false);
			WaitFor(() => { return !server.IsBound; });
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void TestBindTCPServer()
		{
			string validHost = "0.0.0.0";
			ushort port = NetWorker.DEFAULT_PORT;
			int maxConnections = 32;

			server = new TCPServer(maxConnections);

			server.Connect(validHost, port);

			Assert.IsNotNull(server.Me);
			Assert.IsNotNull(server.Me.TcpListenerHandle);

			var endpoint = (IPEndPoint)server.Me.TcpListenerHandle.LocalEndpoint;

			Assert.IsTrue(server.Me.TcpListenerHandle.Server.IsBound);
			Assert.AreEqual(endpoint.Address.ToString(), validHost);
			Assert.AreEqual(endpoint.Port, port);
		}
	}
}

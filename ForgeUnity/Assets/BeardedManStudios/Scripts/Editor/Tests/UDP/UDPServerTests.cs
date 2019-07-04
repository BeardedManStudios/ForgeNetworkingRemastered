using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.UDP
{
	[TestFixture]
	public class UDPServerTests : BaseTest
	{
		private static ushort currentPort = 15937;
		private UDPServer server = null;

		[TearDown]
		public void DisposeServer()
		{
			server.Disconnect(false);
			WaitFor(() => { return !server.IsBound; });
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void TestBindUDPServer()
		{
			currentPort = BaseUDPTests.GetPort();
			string validHost = "0.0.0.0";
			int maxConnections = 32;

			server = new UDPServer(maxConnections);

			server.Connect(validHost, currentPort);

			Assert.IsNotNull(server.Me);
			Assert.IsNotNull(server.Me.IPEndPointHandle);

			var endpoint = server.Me.IPEndPointHandle;

			Assert.IsTrue(server.Client.Client.IsBound);
			Assert.AreEqual(endpoint.Address.ToString(), validHost);
			Assert.AreEqual(endpoint.Port, currentPort);

			server.Disconnect(false);

			Assert.IsFalse(server.IsConnected);
			Assert.IsFalse(server.IsBound);
		}
	}
}

using BeardedManStudios.Forge.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitTests.Source.UDP;

namespace UnitTests.UDP
{
	[TestClass]
	public class UDPClientTests : BaseTest
	{
		private static ushort currentPort;
		private static UDPServer server = null;
		private static UDPClient client = null;

		[ClassInitialize]
		public static void CreateServer(TestContext context)
		{
			currentPort = BaseUDPTests.GetPort();

			server = new UDPServer(32);
			server.Connect(port: currentPort);
			Console.WriteLine("Using port number: " + currentPort);
		}

		[ClassCleanup]
		public static void DisposeServer()
		{
			server.Disconnect(false);
			WaitFor(() => { return !server.IsBound; });
		}

		[TestMethod]
		public void ConnectTest()
		{
			client = new UDPClient();

			Console.WriteLine("Using port number: " + currentPort);
			client.Connect("127.0.0.1", currentPort);
			Assert.IsTrue(client.IsBound);
			WaitFor(() => { return client.IsConnected; });

			client.Disconnect(false);

			WaitFor(() => { return !client.IsConnected; });
			Assert.IsFalse(client.IsBound);
		}
	}
}
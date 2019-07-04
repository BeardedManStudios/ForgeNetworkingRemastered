using System;
using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.UDP
{
	[TestFixture]
	public class UDPClientTests : BaseTest
	{
		private static ushort currentPort;
		private static UDPServer server = null;
		private static UDPClient client = null;

		[OneTimeSetUp]
		public static void CreateServer()
		{
			currentPort = BaseUDPTests.GetPort();

			server = new UDPServer(32);
			server.Connect(port: currentPort);
			Console.WriteLine("Using port number: " + currentPort);
		}

		[OneTimeTearDown]
		public static void DisposeServer()
		{
			server.Disconnect(false);
			WaitFor(() => { return !server.IsBound; });
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
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

using System;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.UDP
{
	[TestFixture]
	public class MultiClientTests : BaseTest
	{
		private static ushort currentPort;
		private static UDPServer server = null;

		[SetUp]
		public void CreateServer()
		{
			currentPort = BaseUDPTests.GetPort();

			server = new UDPServer(32);
			server.Connect(port: currentPort);
			Console.WriteLine("Using port number: " + currentPort);
		}

		[TearDown]
		public void DisposeServer()
		{
			server.Disconnect(false);
			WaitFor(() => { return !server.IsBound; });
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void Connect2ClientsTest()
		{
			var client1 = new UDPClient();
			var client2 = new UDPClient();

			Console.WriteLine("Using port number: " + currentPort);

			client1.Connect("127.0.0.1", currentPort);
			Assert.IsTrue(client1.IsBound);
			WaitFor(() => { return client1.IsConnected; });

			client2.Connect("127.0.0.1", currentPort);
			Assert.IsTrue(client2.IsBound);
			WaitFor(() => { return client2.IsConnected; });

			client1.Disconnect(false);
			client2.Disconnect(false);

			WaitFor(() => { return !client1.IsConnected; });
			Assert.IsFalse(client1.IsBound);

			WaitFor(() => { return !client2.IsConnected; });
			Assert.IsFalse(client2.IsBound);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void Connect3ClientsAtOnceTest()
		{
			int i, clientCount = 3;
			var clients = new List<UDPClient>(clientCount);

			for (i = 0; i < clientCount; i++)
				clients.Add(new UDPClient());

			for (i = 0; i < clients.Count; i++)
			{
				clients[i].Connect("127.0.0.1", currentPort);
				Assert.IsTrue(clients[i].IsBound);
				WaitFor(() => { return clients[i].IsConnected; });
			}

			for (i = 0; i < clients.Count; i++)
				clients[i].Disconnect(false);

			WaitFor(() => { return clients.Count(c => c.IsConnected) == 0; });

			for (i = 0; i < clients.Count; i++)
				Assert.IsFalse(clients[i].IsBound);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void MultipleReconnectTest()
		{
			UDPClient singleClient;
			int i, clientCount = 4;
			for (i = 0; i < clientCount; i++)
			{
				singleClient = new UDPClient();
				singleClient.Connect("127.0.0.1", currentPort);
				Assert.IsTrue(singleClient.IsBound);
				WaitFor(() => { return singleClient.IsConnected; });
				singleClient.Disconnect(false);
				WaitFor(() => { return !singleClient.IsConnected; });
				Assert.IsFalse(singleClient.IsBound);
				singleClient = null;
				WaitFor(() => { return server.Players.Count == 1; });
			}
		}
	}
}

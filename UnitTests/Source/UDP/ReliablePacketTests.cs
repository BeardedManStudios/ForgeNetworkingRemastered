using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTests.Source.UDP
{
	[TestClass]
	public class ReliablePacketTests : BaseUDPTests
	{
		private const int BINARY_GROUP_ID = MessageGroupIds.START_OF_GENERIC_IDS + 1;
		private const string MESSAGE = "THIS IS A TEST TO MAKE SURE BINARY IS WORKING!";

		private static NetworkingPlayer responsePlayer;
		private static string response;
		private static int responseCounter = 0;

		[ClassInitialize]
		public static void CreateServer(TestContext context)
		{
			ConnectSetup();

			server.binaryMessageReceived += BinaryMessageRead;
			client.binaryMessageReceived += BinaryMessageRead;
		}

		private static void BinaryMessageRead(NetworkingPlayer player, Binary frame, NetWorker sender)
		{
			responsePlayer = player;
			response = ObjectMapper.Instance.Map<string>(frame.StreamData);
			responseCounter++;
		}

		[ClassCleanup]
		public static void DisposeServer()
		{
			server.binaryMessageReceived -= BinaryMessageRead;
			client.binaryMessageReceived -= BinaryMessageRead;

			ConnectTeardown();
		}

		private Binary SendBinary(NetWorker networker)
		{
			BMSByte data = new BMSByte();
			ObjectMapper.Instance.MapBytes(data, MESSAGE);

			ulong timestep = (ulong)(DateTime.UtcNow - start).TotalMilliseconds;
			return new Binary(timestep, networker is TCPClient, data, Receivers.Target, 17931, networker is BaseTCP);

		}

		[TestMethod]
		public void SendBinaryReliablyTest()
		{
			WaitFor(() => { return client.IsConnected; });

			server.PacketLossSimulation = 0.95f;
			client.PacketLossSimulation = 0.95f;

			client.Send(SendBinary(client), true);

			WaitFor(() => { return !string.IsNullOrEmpty(response); });
			Assert.AreEqual(server.Players.Last(), responsePlayer);
			Assert.AreEqual(MESSAGE, response);

			responsePlayer = null;
			response = null;

			server.Send(SendBinary(server), false);

			WaitFor(() => { return !string.IsNullOrEmpty(response); });
			Assert.AreEqual(client.Server, responsePlayer);
			Assert.AreEqual(MESSAGE, response);

			response = null;
		}

		[TestMethod]
		public void SendManyBinaryReliablyTest()
		{
			int packetCount = 100;
			responseCounter = 0;

			WaitFor(() => { return client.IsConnected; });

			for (int i = 0; i < packetCount; i++)
			{
				server.Send(SendBinary(server), true);
			}

			WaitFor(() => { return responseCounter == packetCount; });
			int x = 0;
		}
	}
}
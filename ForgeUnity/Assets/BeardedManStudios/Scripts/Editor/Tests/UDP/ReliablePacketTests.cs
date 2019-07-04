using System;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.UDP
{
	[TestFixture]
	public class ReliablePacketTests : BaseUDPTests
	{
		private const int BINARY_GROUP_ID = MessageGroupIds.START_OF_GENERIC_IDS + 1;
		private const string MESSAGE = "THIS IS A TEST TO MAKE SURE BINARY IS WORKING!";

		private static NetworkingPlayer responsePlayer;
		private static string response;
		private static int responseCounter = 0;

		[OneTimeSetUp]
		public static void CreateServer()
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

		[OneTimeTearDown]
		public static void DisposeServer()
		{
			server.binaryMessageReceived -= BinaryMessageRead;
			client.binaryMessageReceived -= BinaryMessageRead;

			ConnectTeardown();
		}

		private Binary SendBinary(NetWorker networker)
		{
			BMSByte data = ObjectMapper.BMSByte(MESSAGE);

			ulong timestep = (ulong)(DateTime.UtcNow - start).TotalMilliseconds;
			return new Binary(timestep, networker is TCPClient, data, Receivers.Target, 17931, networker is BaseTCP);

		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
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
			Assert.AreEqual(client.ServerPlayer, responsePlayer);
			Assert.AreEqual(MESSAGE, response);

			response = null;
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
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
		}
	}
}

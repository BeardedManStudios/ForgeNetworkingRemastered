using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UnitTests.Source.UDP;

namespace UnitTests
{
	[TestClass]
	public class FrameTests : BaseUDPTests
	{
		private const int PORT = 17931;
		private const int BINARY_GROUP_ID = MessageGroupIds.START_OF_GENERIC_IDS + 1;
		private const string MESSAGE = "THIS IS A TEST TO MAKE SURE BINARY IS WORKING!";

		private static NetworkingPlayer responsePlayer;
		private static string response;

		[ClassInitialize]
		public static void CreateServer(TestContext context)
		{
			ConnectSetup();

			server.binaryMessageReceived += BinaryMessageRead;
			client.binaryMessageReceived += BinaryMessageRead;
			server.textMessageReceived += TextMessageRead;
			client.textMessageReceived += TextMessageRead;
		}

		private static void BinaryMessageRead(NetworkingPlayer player, Binary frame, NetWorker sender)
		{
			responsePlayer = player;
			response = ObjectMapper.Instance.Map<string>(frame.StreamData);
		}

		private static void TextMessageRead(NetworkingPlayer player, Text frame, NetWorker sender)
		{
			responsePlayer = player;
			response = frame.ToString();
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

		private Text SendText(NetWorker networker)
		{
			ulong timestep = (ulong)(DateTime.UtcNow - start).TotalMilliseconds;
			return Text.CreateFromString(timestep, MESSAGE, networker is TCPClient, Receivers.Target, 17932, networker is BaseTCP);
		}

		[TestMethod]
		public void SendBinaryTest()
		{
			WaitFor(() => { return client.IsConnected; });

			response = null;
			client.Send(SendBinary(client), false);

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
		public void SendTextTest()
		{
			WaitFor(() => { return client.IsConnected; });

			response = null;
			client.Send(SendText(client), false);

			WaitFor(() => { return !string.IsNullOrEmpty(response); });
			Assert.AreEqual(server.Players.Last(), responsePlayer);
			Assert.AreEqual(MESSAGE, response);

			responsePlayer = null;
			response = null;

			server.Send(SendText(server), false);

			WaitFor(() => { return !string.IsNullOrEmpty(response); });
			Assert.AreEqual(client.Server, responsePlayer);
			Assert.AreEqual(MESSAGE, response);

			response = null;
		}
	}
}
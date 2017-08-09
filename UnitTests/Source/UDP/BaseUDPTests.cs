
using BeardedManStudios.Forge.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Source.UDP
{
	public class BaseUDPTests : BaseTest
	{
		private static List<ushort> usedPorts = new List<ushort>();
		private static ushort currentPort;

		protected static UDPServer server = null;
		protected static UDPClient client = null;
		protected static UDPClient otherClient = null;
		protected static DateTime start;

		public static ushort GetPort()
		{
			ushort port;

			do
			{
				port = (ushort)(new Random().Next(5000, ushort.MaxValue));
			} while (usedPorts.Contains(port));

			usedPorts.Add(port);

			return port;
		}

		protected static void ConnectSetup(bool useOtherClient = false, NetworkObject.NetworkObjectEvent objectCreatedCallback = null)
		{
			currentPort = GetPort();
			start = DateTime.UtcNow;

			server = new UDPServer(32);
			server.Connect(port: currentPort);

			if (objectCreatedCallback != null)
				server.objectCreated += objectCreatedCallback;

			client = new UDPClient();
			client.Connect("127.0.0.1", currentPort);

			if (objectCreatedCallback != null)
				client.objectCreated += objectCreatedCallback;

			WaitFor(() => { return server.Players.Last().Accepted; });
			NetworkObject.Flush(client);

			if (!useOtherClient)
				return;

			OtherClientConnectSetup(objectCreatedCallback);
		}

		protected static void OtherClientConnectSetup(NetworkObject.NetworkObjectEvent objectCreatedCallback = null)
		{
			otherClient = new UDPClient();
			otherClient.Connect("127.0.0.1", currentPort);

			if (objectCreatedCallback != null)
				otherClient.objectCreated += objectCreatedCallback;

			WaitFor(() => { return server.Players.Last().Accepted; });
			NetworkObject.Flush(otherClient);
		}

		protected static void ConnectTeardown(NetworkObject.NetworkObjectEvent objectCreatedCallback = null)
		{
			if (objectCreatedCallback != null)
			{
				client.objectCreated -= objectCreatedCallback;
				server.objectCreated -= objectCreatedCallback;
			}

			if (otherClient != null)
			{
				if (objectCreatedCallback != null)
					otherClient.objectCreated -= objectCreatedCallback;

				otherClient.Disconnect(false);
			}

			client.Disconnect(false);
			server.Disconnect(false);

			WaitFor(() => { return !client.IsBound && !server.IsBound && (otherClient == null || !otherClient.IsBound); });

			server = null;
			client = null;
			otherClient = null;
		}
	}
}

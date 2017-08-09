/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.SimpleJSON;

namespace BeardedManStudios.Forge.Networking.Nat
{
	public class NatHolePunch
	{
		/// <summary>
		/// This is the default server port for Forge Networking's NAT hole punch server
		/// </summary>
		public const ushort DEFAULT_NAT_SERVER_PORT = 15941;

		/// <summary>
		/// An delegate that can be used to structure any kind of nat event
		/// </summary>
		/// <param name="host">The host address for the request</param>
		/// <param name="port">The port number for the request</param>
		public delegate void NatEvent(string host, ushort port);

		/// <summary>
		/// Occurs when a client is attempting to connect to a server
		/// </summary>
		public event NatEvent clientConnectAttempt;

		/// <summary>
		/// The client that is used to connect to the remote NAT hole punch server
		/// </summary>
		public UDPClient Client { get; private set; }

		/// <summary>
		/// Connect to the NAT hole punch server, this is called from a client machine
		/// </summary>
		/// <param name="host">The host address of the server the client is trying to connect to</param>
		/// <param name="port">The port number of the server the client is trying to connect to</param>
		/// <param name="clientPort">The port number that the client is listening on</param>
		/// <param name="natServer">The NAT server host address to connect to</param>
		/// <param name="natPort">The NAT server port number to connect to</param>
		public void Connect(string host, ushort port, ushort clientPort, string natServer, ushort natPort = DEFAULT_NAT_SERVER_PORT)
		{
			// Don't allow multiple NAT server connection requests at once
			if (Client != null)
				return;

			// Connect to the NAT server
			Client = new UDPClient();
			Client.Connect(natServer, natPort, pendCreates: true);

			NetWorker.BaseNetworkEvent accepted = (NetWorker sender) =>
			{
				// Send the data to the nat server with the host address and port that this client
				// is trying to connect to so that it can punch a hole in the network for this client
				JSONNode sendJson = JSONNode.Parse("{}");
				sendJson.Add("host", new JSONData(host));
				sendJson.Add("port", new JSONData(port));
				sendJson.Add("clientPort", new JSONData(clientPort));

				// Send the message to the NAT server
				Text connect = Text.CreateFromString(Client.Time.Timestep, sendJson.ToString(), false, Receivers.Server, MessageGroupIds.NAT_SERVER_CONNECT, false);
				Client.Send(connect, true);
				Client.messageConfirmed += (player, packet) => { if (packet.uniqueId == connect.UniqueId) { Client.Disconnect(false); } };
			};

			Client.serverAccepted += accepted;
		}

		/// <summary>
		/// Registers a server to the NAT server so that it can be requested to be joined by clients
		/// </summary>
		/// <param name="currentPort">The current port that this server is listening for client connections on</param>
		/// <param name="natServer">The NAT server host address to connect to</param>
		/// <param name="natPort">The NAT server port number to connect to</param>
		public void Register(ushort currentPort, string natServer, ushort natPort = DEFAULT_NAT_SERVER_PORT)
		{
			// Don't allow multiple NAT server connection requests at once
			if (Client != null)
				return;

			// Connect to the NAT server
			Client = new UDPClient();
			Client.Connect(natServer, natPort, pendCreates: true);

			// When connected, request for this server to be registered to the NAT lookup for clients
			NetWorker.BaseNetworkEvent accepted = (NetWorker sender) =>
			{
				JSONNode obj = JSONNode.Parse("{}");
				obj.Add("port", new JSONData(currentPort));

				JSONClass sendJson = new JSONClass();
				sendJson.Add("register", obj);

				// Send the message to the NAT server
				Text register = Text.CreateFromString(Client.Time.Timestep, sendJson.ToString(), false, Receivers.Target, MessageGroupIds.NAT_SERVER_REGISTER, false);
				Client.Send(register, true);
			};

			Client.serverAccepted += accepted;

			// Setup the callback events for when clients attempt to join
			Client.textMessageReceived += PlayerConnectRequestReceived;
		}

		/// <summary>
		/// The callback for when a player is trying to connect to this server from
		/// the NAT server
		/// </summary>
		/// <param name="player">The NAT server player</param>
		/// <param name="frame">The data that the NAT server has sent for consumption</param>
		private void PlayerConnectRequestReceived(NetworkingPlayer player, Text frame, NetWorker sender)
		{
			Logging.BMSLog.Log("PLAYER CONNECTION REQUEST");
			Logging.BMSLog.Log(frame.ToString());

			try
			{
				// This is a Text frame with JSON so parse it all
				var json = JSON.Parse(frame.ToString());

				// If this is a route from the NAT then read it
				if (json["nat"] != null)
				{
					Logging.BMSLog.Log("DOING NAT");
					// These fields are required for this server to punch a hole for a client
					if (json["nat"]["host"] != null && json["nat"]["port"] != null)
					{
						string host = json["nat"]["host"];
						ushort port = json["nat"]["port"].AsUShort;
						//Logging.BMSLog.Log($"HOST IS {host} AND PORT IS {port}");

						// Fire the event that a client is trying to connect
						if (clientConnectAttempt != null)
							clientConnectAttempt(host, port);
					}
				}
			}
			catch { /* Ignore message */ }
		}

		public void Disconnect()
		{
			if (Client == null)
				return;

			Client.Disconnect(false);
		}
	}
}
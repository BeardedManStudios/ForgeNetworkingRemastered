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
using BeardedManStudios.Forge.Networking.Nat;
using BeardedManStudios.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class UDPServer : BaseUDP, IServer
	{
		public Dictionary<string, UDPNetworkingPlayer> udpPlayers = new Dictionary<string, UDPNetworkingPlayer>();

		private UDPNetworkingPlayer currentReadingPlayer = null;

		private List<UDPPacketComposer> pendingComposers = new List<UDPPacketComposer>();

		public UDPServer(int maxConnections) : base(maxConnections) { AcceptingConnections = true; }

		public NatHolePunch nat = new NatHolePunch();

		protected List<FrameStream> bufferedMessages = new List<FrameStream>();

		/// <summary>
		/// Used to determine if this server is currently accepting connections
		/// </summary>
		public bool AcceptingConnections { get; private set; }

		public void Send(NetworkingPlayer player, FrameStream frame, bool reliable = false)
		{
			UDPPacketComposer composer = new UDPPacketComposer(this, player, frame, reliable);

			// If this message is reliable then make sure to keep a reference to the composer
			// so that there are not any run-away threads
			if (reliable)
			{
				lock (pendingComposers)
				{
					// Use the completed event to clean up the object from memory
					composer.completed += ComposerCompleted;
					pendingComposers.Add(composer);
				}
			}
		}

		public override void Send(FrameStream frame, bool reliable = false)
		{
			Send(frame, reliable, null);
		}

		public void Send(FrameStream frame, bool reliable = false, NetworkingPlayer skipPlayer = null)
		{
			if (frame.Receivers == Receivers.AllBuffered || frame.Receivers == Receivers.OthersBuffered)
				bufferedMessages.Add(frame);

			lock (Players)
			{
				foreach (NetworkingPlayer player in Players)
				{
					// Don't send messages to a player who has not been accepted by the server yet
					if ((!player.Accepted && !player.PendingAccpeted) || player == skipPlayer)
						continue;

					if (player == frame.Sender)
					{
						// Don't send a message to the sending player if it was meant for others
						if (frame.Receivers == Receivers.Others || frame.Receivers == Receivers.OthersBuffered || frame.Receivers == Receivers.OthersProximity)
							continue;
					}

					// Check to see if the request is based on proximity
					if (frame.Receivers == Receivers.AllProximity || frame.Receivers == Receivers.OthersProximity)
					{
						// If the target player is not in the same proximity zone as the sender
						// then it should not be sent to that player
						if (player.ProximityLocation.Distance(frame.Sender.ProximityLocation) > ProximityDistance)
						{
							continue;
						}
					}

					try
					{
						Send(player, frame, reliable);
					}
					catch
					{
						Disconnect(player, true);
					}
				}
			}
		}

		/// <summary>
		/// Used to clean up the target composer from memory
		/// </summary>
		/// <param name="composer">The composer that has completed</param>
		private void ComposerCompleted(UDPPacketComposer composer)
		{
			lock (pendingComposers)
			{
				pendingComposers.Remove(composer);
			}
		}

		public void Connect(string host = "0.0.0.0", ushort port = DEFAULT_PORT, string natHost = "", ushort natPort = NatHolePunch.DEFAULT_NAT_SERVER_PORT)
		{
			try
			{
				Client = new CachedUdpClient(port);
				Client.EnableBroadcast = true;
				Me = new NetworkingPlayer(ServerPlayerCounter++, host, true, ResolveHost(host, port), this);
				Me.InstanceGuid = InstanceGuid.ToString();

				// Create the thread that will be listening for new data from connected clients and start its execution
				Task.Queue(ReadClients);

				// Create the thread that will check for player timeouts
				Task.Queue(CheckClientTimeout);

				// Do any generic initialization in result of the successful bind
				OnBindSuccessful();

				//Let myself know I connected successfully
				OnPlayerConnected(Me);
				// Set myself as a connected client
				Me.Connected = true;

				//Set the port
				SetPort((ushort)((IPEndPoint)Client.Client.LocalEndPoint).Port);

				if (!string.IsNullOrEmpty(natHost))
				{
					nat.Register((ushort)Me.IPEndPointHandle.Port, natHost, natPort);
					nat.clientConnectAttempt += NatClientConnectAttempt;
				}
			}
			catch (Exception e)
			{
				Logging.BMSLog.LogException(e);
				// Do any generic initialization in result of the binding failure
				OnBindFailure();

				throw new FailedBindingException("Failed to bind to host/port, see inner exception", e);
			}
		}

		/// <summary>
		/// Disconnects this server and all of it's clients
		/// </summary>
		/// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
		public override void Disconnect(bool forced)
		{
			nat.Disconnect();

			// Since we are disconnecting we need to stop the read thread
			readThreadCancel = true;

			lock (Players)
			{
				// Go through all of the players and disconnect them
				foreach (NetworkingPlayer player in Players)
				{
					if (player != Me)
						Disconnect(player, forced);
				}

				CleanupDisconnections();

				int counter = 0;
				for (; ; counter++)
				{
					if (counter >= 10 || Players.Count == 1)
						break;

					Thread.Sleep(100);
				}

				// Send signals to the methods registered to the disconnect events
				if (!forced)
					OnDisconnected();
				else
					OnForcedDisconnect();

				// Stop listening for new connections
				Client.Close();
			}
		}

		/// <summary>
		/// Disconnects a client
		/// </summary>
		/// <param name="client">The target client to be disconnected</param>
		public void Disconnect(NetworkingPlayer player, bool forced)
		{
			if (!forced)
				DisconnectingPlayers.Add(player);
			else
				ForcedDisconnectingPlayers.Add(player);
		}

		/// <summary>
		/// Call the base disconnect pending method to remove all pending disconnecting clients
		/// </summary>
		private void CleanupDisconnections() { DisconnectPending(RemovePlayer); }

		/// <summary>
		/// Fully remove the player from the network
		/// </summary>
		/// <param name="player">The target player</param>
		/// <param name="forced">If the player is being forcibly removed from an exception</param>
		private void RemovePlayer(NetworkingPlayer player, bool forced)
		{
			// Tell the player that they are getting disconnected
			Send(player, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, false), true);

			Thread.Sleep(500);

			OnPlayerDisconnected(player);
			udpPlayers.Remove(player.Ip + "+" + player.Port);
		}

		/// <summary>
		/// Checks all of the clients to see if any of them are timed out
		/// </summary>
		private void CheckClientTimeout()
		{
			List<NetworkingPlayer> timedoutPlayers = new List<NetworkingPlayer>();
			while (IsBound)
			{
				IteratePlayers((player) =>
				{
					// Don't process the server during this check
					if (player == Me)
						return;

					if (player.TimedOut())
					{
						timedoutPlayers.Add(player);
					}
				});

				if (timedoutPlayers.Count > 0)
				{
					foreach (NetworkingPlayer player in timedoutPlayers)
					{
						Disconnect(player, true);
						OnPlayerTimeout(player);
						CleanupDisconnections();
					}

					timedoutPlayers.Clear();
				}

				// Wait a second before checking again
				Thread.Sleep(1000);
			}
		}

		/// <summary>
		/// Infinite loop listening for new data from all connected clients on a separate thread.
		/// This loop breaks when readThreadCancel is set to true
		/// </summary>
		private void ReadClients()
		{
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);
			string incomingEndpoint = string.Empty;

			BMSByte packet = null;

			// Intentional infinite loop
			while (IsBound)
			{
				// If the read has been flagged to be canceled then break from this loop
				if (readThreadCancel)
					return;

				try
				{
					// Read a packet from the network
					packet = Client.Receive(ref groupEP, ref incomingEndpoint);

					if (PacketLossSimulation > 0.0f && new Random().NextDouble() <= PacketLossSimulation)
					{
						// Skip this message
						continue;
					}

					BandwidthIn += (ulong)packet.Size;
				}
				catch
				{
					if (udpPlayers.ContainsKey(incomingEndpoint))
					{
						Disconnect(udpPlayers[incomingEndpoint], true);
						CleanupDisconnections();
					}

					continue;
				}

				// Check to make sure a message was received
				if (packet == null || packet.Size <= 0)
					continue;

				if (!udpPlayers.ContainsKey(incomingEndpoint))
				{
					SetupClient(packet, incomingEndpoint, groupEP);
					continue;
				}
				else
				{
					currentReadingPlayer = udpPlayers[incomingEndpoint];

					if (!currentReadingPlayer.Accepted && !currentReadingPlayer.PendingAccpeted)
					{
						// It is possible that the response validation was dropped so
						// check if the client is resending for a response
						byte[] response = Websockets.ValidateConnectionHeader(packet.CompressBytes());

						// The client has sent the connection request again
						if (response != null)
						{
							Client.Send(response, response.Length, groupEP);
							continue;
						}
						else
						{
							currentReadingPlayer.PendingAccpeted = true;
							ReadPacket(packet);
						}
					}
					else
					{
						currentReadingPlayer.Ping();
						ReadPacket(packet);
					}
				}
			}
		}

		private void SetupClient(BMSByte packet, string incomingEndpoint, IPEndPoint groupEP)
		{
			// Check for a local listing request
			if (packet.Size.Between(2, 4) && packet[0] == BROADCAST_LISTING_REQUEST_1 && packet[1] == BROADCAST_LISTING_REQUEST_2 && packet[2] == BROADCAST_LISTING_REQUEST_3)
			{
				// This may be a local listing request so respond with the server flag byte
				Client.Send(new byte[] { SERVER_BROADCAST_CODE }, 1, groupEP);
				return;
			}

			if (Players.Count == MaxConnections)
			{
				// Tell the client why they are being disconnected
				Send(Error.CreateErrorMessage(Time.Timestep, "Max Players Reached On Server", false, MessageGroupIds.MAX_CONNECTIONS, true));

				// Send the close connection frame to the client
				Send(new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, false));

				return;
			}
			else if (!AcceptingConnections)
			{
				// Tell the client why they are being disconnected
				Send(Error.CreateErrorMessage(Time.Timestep, "The server is busy and not accepting connections", false, MessageGroupIds.MAX_CONNECTIONS, true));

				// Send the close connection frame to the client
				Send(new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, false));

				return;
			}

			// Validate that the connection headers are properly formatted
			byte[] response = Websockets.ValidateConnectionHeader(packet.CompressBytes());

			// The response will be null if the header sent is invalid, if so then disconnect client as they are sending invalid headers
			if (response == null)
				return;

			UDPNetworkingPlayer player = new UDPNetworkingPlayer(ServerPlayerCounter++, incomingEndpoint, false, groupEP, this);

			// If all is in order then send the validated response to the client
			Client.Send(response, response.Length, groupEP);

			OnPlayerConnected(player);
			udpPlayers.Add(incomingEndpoint, player);

			// The player has successfully connected
			player.Connected = true;
		}

		private void ReadPacket(BMSByte packet)
		{
			if (packet.Size < 17)
				return;

			try
			{
				// Format the byte data into a UDPPacket struct
				UDPPacket formattedPacket = TranscodePacket(currentReadingPlayer, packet);

				// Check to see if this is a confirmation packet, which is just
				// a packet to say that the reliable packet has been read
				if (formattedPacket.isConfirmation)
				{
					// Called once the player has confirmed that it has been accepted
					if (formattedPacket.groupId == MessageGroupIds.NETWORK_ID_REQUEST && !currentReadingPlayer.Accepted)
					{
						System.Diagnostics.Debug.WriteLine(string.Format("[{0}] REQUESTED ID RECEIVED", DateTime.Now.Millisecond));
						// The player has been accepted
						OnPlayerAccepted(currentReadingPlayer);
					}

					OnMessageConfirmed(currentReadingPlayer, formattedPacket);
					return;
				}

				// Add the packet to the manager so that it can be tracked and executed on complete
				currentReadingPlayer.PacketManager.AddPacket(formattedPacket, PacketSequenceComplete);
			}
			catch (Exception ex)
			{
				Logging.BMSLog.LogException(ex);

				// The player is sending invalid data so disconnect them
				Disconnect(currentReadingPlayer, true);
			}
		}

		private void PacketSequenceComplete(BMSByte data, int groupId, byte receivers)
		{
			// Pull the frame from the sent message
			FrameStream frame = Factory.DecodeMessage(data.CompressBytes(), false, groupId, currentReadingPlayer, receivers);

			// Check for default messages
			if (frame is Text)
			{
				// This packet is sent if the player did not receive it's network id
				if (frame.GroupId == MessageGroupIds.NETWORK_ID_REQUEST)
				{
					currentReadingPlayer.InstanceGuid = frame.ToString();

					OnPlayerGuidAssigned(currentReadingPlayer);

					// If so, just resend the player id
					writeBuffer.Clear();
					writeBuffer.Append(BitConverter.GetBytes(currentReadingPlayer.NetworkId));
					Send(currentReadingPlayer, new Binary(Time.Timestep, false, writeBuffer, Receivers.Target, MessageGroupIds.NETWORK_ID_REQUEST, false), true);

					SendBuffer(currentReadingPlayer);
					return;
				}
			}

			if (frame is ConnectionClose)
			{
				Send(currentReadingPlayer, new ConnectionClose(Time.Timestep, false, Receivers.Server, MessageGroupIds.DISCONNECT, false), false);

				Disconnect(currentReadingPlayer, false);
				CleanupDisconnections();
				return;
			}

			// Send an event off that a packet has been read
			OnMessageReceived(currentReadingPlayer, frame);
		}

		/// <summary>
		/// A callback from the NatHolePunch object saying that a client is trying to connect
		/// </summary>
		/// <param name="host">The host address of the client trying to connect</param>
		/// <param name="port">The port number to communicate with the client on</param>
		private void NatClientConnectAttempt(string host, ushort port)
		{
			var x = ResolveHost(host, port);
			Logging.BMSLog.LogFormat("ATTEMPTING CONNECT ON {0} AND PORT IS {1}", host, port);
			Logging.BMSLog.LogFormat("RESOLVED IS {0} AND {1}", x.Address.ToString(), x.Port);

			// Punch a hole in the nat for this client
			Client.Send(new byte[1] { 0 }, 1, ResolveHost(host, port));
		}

		private void SendBuffer(NetworkingPlayer player)
		{
			foreach (FrameStream frame in bufferedMessages)
				Send(player, frame, true);
		}

		public override void Ping()
		{
			//I am the server, so 0 ms...
			OnPingRecieved(0);
		}

		/// <summary>
		/// Pong back to the client
		/// </summary>
		/// <param name="playerRequesting"></param>
		protected override void Pong(NetworkingPlayer playerRequesting, DateTime time)
		{
			Send(playerRequesting, GeneratePong(time));
		}

		public void StopAcceptingConnections()
		{
			AcceptingConnections = false;
		}

		public void StartAcceptingConnections()
		{
			AcceptingConnections = true;
		}
	}
}
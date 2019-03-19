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
using System.Linq;
using System.Net;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class UDPServer : BaseUDP, IServer
	{
		private CommonServerLogic commonServerLogic;

		public Dictionary<string, UDPNetworkingPlayer> udpPlayers = new Dictionary<string, UDPNetworkingPlayer>();

		private UDPNetworkingPlayer currentReadingPlayer = null;

		public UDPServer(int maxConnections) : base(maxConnections)
		{
			AcceptingConnections = true;
			BannedAddresses = new List<string>();
			commonServerLogic = new CommonServerLogic(this);
		}

		public NatHolePunch nat = new NatHolePunch();

		protected List<FrameStream> bufferedMessages = new List<FrameStream>();

		public List<string> BannedAddresses { get; set; }

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

					if (!commonServerLogic.PlayerIsReceiver(player, frame, ProximityDistance, skipPlayer, ProximityModeUpdateFrequency))
						continue;

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


		// overload for ncw field distance check case
		public void Send(FrameStream frame, NetworkingPlayer sender, bool reliable = false, NetworkingPlayer skipPlayer = null)
		{
			if (frame.Receivers == Receivers.AllBuffered || frame.Receivers == Receivers.OthersBuffered)
				bufferedMessages.Add(frame);

			lock (Players)
			{
				foreach (NetworkingPlayer player in Players)
				{
					// check for distance here so the owner doesn't need to be sent in stream, used for NCW field proximity check
					if (!commonServerLogic.PlayerIsDistanceReceiver(sender, player, frame, ProximityDistance, ProximityModeUpdateFrequency))
						continue;

					if (!commonServerLogic.PlayerIsReceiver(player, frame, ProximityDistance, skipPlayer, ProximityModeUpdateFrequency))
						continue;

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
		/// Sends binary message to the specified receiver(s)
		/// </summary>
		/// <param name="receivers">The client to receive the message</param>
		/// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
		/// <param name="reliable">True if message must be delivered</param>
		/// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
		public virtual void Send(NetworkingPlayer player, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, bool reliable = false, params object[] objectsToSend)
		{
			BMSByte data = ObjectMapper.BMSByte(objectsToSend);
			Binary sendFrame = new Binary(Time.Timestep, false, data, Receivers.Target, messageGroupId, false);
			Send(player, sendFrame, reliable);
		}

		/// <summary>
		/// Sends binary message to all clients ignoring the specific one
		/// </summary>
		/// <param name="receivers">The clients / server to receive the message</param>
		/// <param name="playerToIgnore"></param>
		/// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
		/// <param name="reliable">True if message must be delivered</param>
		/// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
		public virtual void Send(Receivers receivers = Receivers.Target, NetworkingPlayer playerToIgnore = null, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, bool reliable = false, params object[] objectsToSend)
		{
			BMSByte data = ObjectMapper.BMSByte(objectsToSend);
			Binary sendFrame = new Binary(Time.Timestep, false, data, receivers, messageGroupId, false);
			Send(sendFrame, reliable, playerToIgnore);
		}

		public void Connect(string host = "0.0.0.0", ushort port = DEFAULT_PORT, string natHost = "", ushort natPort = NatHolePunch.DEFAULT_NAT_SERVER_PORT)
		{
			if (Disposed)
				throw new ObjectDisposedException("UDPServer", "This object has been disposed and can not be used to connect, please use a new UDPServer");

			try
			{
				Client = new CachedUdpClient(port);
				Client.EnableBroadcast = true;
				Me = new NetworkingPlayer(ServerPlayerCounter++, host, true, ResolveHost(host, port), this);
				Me.InstanceGuid = InstanceGuid.ToString();

				// Do any generic initialization in result of the successful bind
				OnBindSuccessful();

				// Create the thread that will be listening for new data from connected clients and start its execution
				Task.Queue(ReadClients);

				// Create the thread that will check for player timeouts
				Task.Queue(() =>
				{
					commonServerLogic.CheckClientTimeout((player) =>
					{
						Disconnect(player, true);
						OnPlayerTimeout(player);
						CleanupDisconnections();
					});
				});

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
			commonServerLogic.Disconnect(player, forced, DisconnectingPlayers, ForcedDisconnectingPlayers);
		}

		/// <summary>
		/// Call the base disconnect pending method to remove all pending disconnecting clients
		/// </summary>
		private void CleanupDisconnections() { DisconnectPending(RemovePlayer); }

		/// <summary>
		/// Commit the disconnects
		/// </summary>
		public void CommitDisconnects() { CleanupDisconnections(); }

		/// <summary>
		/// Fully remove the player from the network
		/// </summary>
		/// <param name="player">The target player</param>
		/// <param name="forced">If the player is being forcibly removed from an exception</param>
		private void RemovePlayer(NetworkingPlayer player, bool forced)
		{
			lock (Players)
			{
				if (player.IsDisconnecting)
					return;

				player.IsDisconnecting = true;
			}

			// Tell the player that they are getting disconnected
			Send(player, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, false), !forced);

			if (!forced)
			{
				Task.Queue(() =>
				{
					FinalizeRemovePlayer(player, forced);
				}, 1000);
			}
			else
				FinalizeRemovePlayer(player, forced);
		}

		private void FinalizeRemovePlayer(NetworkingPlayer player, bool forced)
		{
			udpPlayers.Remove(player.Ip + "+" + player.Port);
			OnPlayerDisconnected(player);

			if (forced)
				ForcedDisconnectingPlayers.Remove(player);
			else
				DisconnectingPlayers.Remove(player);
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
					UDPNetworkingPlayer player;
					if (udpPlayers.TryGetValue(incomingEndpoint, out player))
					{
						FinalizeRemovePlayer(player, true);
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

					if (!currentReadingPlayer.Accepted && !currentReadingPlayer.PendingAccepted)
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
							currentReadingPlayer.PendingAccepted = true;
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
				// Don't reply if the server is not currently accepting connections
				if (!AcceptingConnections)
					return;

				// This may be a local listing request so respond with the server flag byte
				Client.Send(new byte[] { SERVER_BROADCAST_CODE }, 1, groupEP);
				return;
			}

			if (Players.Count == MaxConnections)
			{
				// Tell the client why they are being disconnected
				var frame = Error.CreateErrorMessage(Time.Timestep, "Max Players Reached On Server", false, MessageGroupIds.MAX_CONNECTIONS, false);
				var playerToDisconnect = new UDPNetworkingPlayer(ServerPlayerCounter++, incomingEndpoint, false, groupEP, this);
				new UDPPacketComposer(this, playerToDisconnect, frame, false);

				// Send the close connection frame to the client
				new UDPPacketComposer(this, playerToDisconnect, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, false), false);

				return;
			}
			else if (!AcceptingConnections)
			{
				// Tell the client why they are being disconnected
				var frame = Error.CreateErrorMessage(Time.Timestep, "The server is busy and not accepting connections", false, MessageGroupIds.NOT_ACCEPT_CONNECTIONS, false);
				var playerToDisconnect = new UDPNetworkingPlayer(ServerPlayerCounter++, incomingEndpoint, false, groupEP, this);
				new UDPPacketComposer(this, playerToDisconnect, frame, false);

				// Send the close connection frame to the client
				new UDPPacketComposer(this, playerToDisconnect, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, false), false);

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

				if (formattedPacket.endPacket && !formattedPacket.isConfirmation)
				{
					// Due to the Forge Networking protocol, the only time that packet 1
					// will be 71 and the second packet be 69 is a forced disconnect reconnect
					if (packet[0] == 71 && packet[1] == 69)
					{
						udpPlayers.Remove(currentReadingPlayer.Ip + "+" + currentReadingPlayer.Port);
						FinalizeRemovePlayer(currentReadingPlayer, true);
						return;
					}
				}

				// Check to see if this is a confirmation packet, which is just
				// a packet to say that the reliable packet has been read
				if (formattedPacket.isConfirmation)
				{
					// Called once the player has confirmed that it has been accepted
					if (formattedPacket.groupId == MessageGroupIds.NETWORK_ID_REQUEST && !currentReadingPlayer.Accepted && currentReadingPlayer.Authenticated)
					{
						System.Diagnostics.Debug.WriteLine(string.Format("[{0}] REQUESTED ID RECEIVED", DateTime.Now.Millisecond));
						// The player has been accepted
						OnPlayerAccepted(currentReadingPlayer);
					}

					OnMessageConfirmed(currentReadingPlayer, formattedPacket);
					return;
				}

				// Add the packet to the manager so that it can be tracked and executed on complete
				currentReadingPlayer.PacketManager.AddPacket(formattedPacket, PacketSequenceComplete, this);
			}
			catch (Exception ex)
			{
				Logging.BMSLog.LogException(ex);

				// The player is sending invalid data so disconnect them
				Disconnect(currentReadingPlayer, true);
			}
		}

		private void PacketSequenceComplete(BMSByte data, int groupId, byte receivers, bool isReliable)
		{
			// Pull the frame from the sent message
			FrameStream frame = Factory.DecodeMessage(data.CompressBytes(), false, groupId, currentReadingPlayer, receivers);

			if (isReliable)
			{
				frame.ExtractReliableId();

				// TODO:  If the current reliable index for this player is not at
				// the specified index, then it needs to wait for the correct ordering
				currentReadingPlayer.WaitReliable(frame);
			}
			else
				FireRead(frame, currentReadingPlayer);
		}

		public override void FireRead(FrameStream frame, NetworkingPlayer currentPlayer)
		{
			// Check for default messages
			if (frame is Text)
			{
				// This packet is sent if the player did not receive it's network id
				if (frame.GroupId == MessageGroupIds.NETWORK_ID_REQUEST)
				{
					currentPlayer.InstanceGuid = frame.ToString();

					bool rejected;
					OnPlayerGuidAssigned(currentPlayer, out rejected);

					// If the player was rejected during the handling of the playerGuidAssigned event, don't accept them.
					if (rejected)
						return;

                    // If so, check if there's a user authenticator
                    if (authenticator != null)
                    {
                        authenticator.IssueChallenge(this, currentPlayer, IssueChallenge, AuthUser);
                    } else
                    {
                        AuthUser(currentPlayer);
                    }
					return;
				} 
			} else if (frame is Binary)
            {
                if (frame.GroupId == MessageGroupIds.AUTHENTICATION_RESPONSE)
                {
                    // Authenticate user response
                    if (currentPlayer.Authenticated || authenticator == null)
                        return;

                    authenticator.VerifyResponse(this, currentPlayer, frame.StreamData, AuthUser, RejectUser);
                    return;
                }
            }

			if (frame is ConnectionClose)
			{
				//Send(currentReadingPlayer, new ConnectionClose(Time.Timestep, false, Receivers.Server, MessageGroupIds.DISCONNECT, false), false);

				Disconnect(currentReadingPlayer, true);
				CleanupDisconnections();
				return;
			}

			// Send an event off that a packet has been read
			OnMessageReceived(currentReadingPlayer, frame);
		}

        /// <summary>
        /// Callback for user auth. Sends an auth challenge to the user.
        /// </summary>
        private void IssueChallenge(NetworkingPlayer player, BMSByte buffer)
        {
            Send(player, new Binary(Time.Timestep, false, buffer, Receivers.Target, MessageGroupIds.AUTHENTICATION_CHALLENGE, false), true);
        }

        /// <summary>
        /// Callback for user auth. Authenticates the user and sends the user their network id for acceptance.
        /// </summary>
        private void AuthUser(NetworkingPlayer player)
        {
            OnPlayerAuthenticated(player);

            // If authenticated, send the player their network id and accept them
            var buffer = new BMSByte();
            buffer.Append(BitConverter.GetBytes(player.NetworkId));
            Send(player, new Binary(Time.Timestep, false, buffer, Receivers.Target, MessageGroupIds.NETWORK_ID_REQUEST, false), true);
            SendBuffer(player);
        }

        /// <summary>
        /// Callback for user auth. Sends an authentication failure message to the user and then disconnects them.
        /// </summary>
        private void RejectUser(NetworkingPlayer player)
        {
            OnPlayerRejected(player);
            Send(player, Error.CreateErrorMessage(Time.Timestep, "Authentication Failed", false, MessageGroupIds.AUTHENTICATION_FAILURE, false), false);
            SendBuffer(player);
            Disconnect(player, true);
            CommitDisconnects();
        }

        /// <summary>
        /// A callback from the NatHolePunch object saying that a client is trying to connect
        /// </summary>
        /// <param name="host">The host address of the client trying to connect</param>
        /// <param name="port">The port number to communicate with the client on</param>
        private void NatClientConnectAttempt(string host, ushort port)
		{
			IPEndPoint clientIPEndPoint;

			Logging.BMSLog.LogFormat("ATTEMPTING CONNECT ON {0} AND PORT IS {1}", host, port);

			try
			{
				clientIPEndPoint = ResolveHost(host, port);
			}
			catch (ArgumentException)
			{
				Logging.BMSLog.LogExceptionFormat("Unable to resolve client host {0}", host);
				// Do nothing as the client's host cannot be resolved.
				return;
			}

			Logging.BMSLog.LogFormat("RESOLVED IS {0} AND {1}", clientIPEndPoint.Address.ToString(), clientIPEndPoint.Port);

			// Punch a hole in the nat for this client
			Client.Send(new byte[1] { 0 }, 1, clientIPEndPoint);
		}

		private void SendBuffer(NetworkingPlayer player)
		{
			foreach (FrameStream frame in bufferedMessages)
				Send(player, frame, true);
		}

		public override void Ping()
		{
			// I am the server, so 0 ms...
			OnPingRecieved(0, Me);
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

		public void BanPlayer(ulong networkId, int minutes)
		{
			NetworkingPlayer player = Players.FirstOrDefault(p => p.NetworkId == networkId);

			if (player == null)
				return;

			BannedAddresses.Add(player.Ip);
			Disconnect(player, true);
			CommitDisconnects();
		}
	}
}

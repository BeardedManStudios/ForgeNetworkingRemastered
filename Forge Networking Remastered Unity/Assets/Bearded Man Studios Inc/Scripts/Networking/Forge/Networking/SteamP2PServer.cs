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

#if STEAMWORKS
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Steamworks;

namespace BeardedManStudios.Forge.Networking
{
	public class SteamP2PServer : BaseSteamP2P, IServer
	{
		private CommonServerLogic commonServerLogic;

		public Dictionary<CSteamID, SteamNetworkingPlayer> steamPlayers = new Dictionary<CSteamID, SteamNetworkingPlayer>();

		private SteamNetworkingPlayer currentReadingPlayer = null;

#region Steamworks API
        protected Callback<P2PSessionRequest_t> m_CallbackP2PSessionRequest;
        protected Callback<P2PSessionConnectFail_t> m_CallbackP2PSessionConnectFail;
        protected Callback<LobbyCreated_t> m_CallbackLobbyCreated;

        //-----------------------------------------------------------------------------
        // Purpose: Handle clients connecting
        //-----------------------------------------------------------------------------
        void OnP2PSessionRequest(P2PSessionRequest_t pCallback)
        {
            Logging.BMSLog.Log("OnP2PSesssionRequest Called steamIDRemote: " + pCallback.m_steamIDRemote);
            // we'll accept a connection from anyone
            if(AcceptingConnections)
            {
                SteamNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);
            }
        }

        //-----------------------------------------------------------------------------
        // Purpose: Handle clients disconnecting
        //-----------------------------------------------------------------------------
        void OnP2PSessionConnectFail(P2PSessionConnectFail_t data)
        {
            if (data.m_eP2PSessionError > 0)
            {
                string e = string.Empty;

                Logging.BMSLog.Log("OnP2PSessionConnectFail Called steamIDRemote: " + data.m_steamIDRemote);
                Logging.BMSLog.Log("OnP2PSessionConnectFail error: [" + data.m_eP2PSessionError + "]"); // Riley
                Disconnect(steamPlayers[data.m_steamIDRemote], true);
                switch (data.m_eP2PSessionError)
                {
                    case 1:
                        e = "The target user is not running the same game."; break;
                    case 2:
                        e = "The local user doesn't own the app that is running."; break;
                    case 3:
                        e = "Target user isn't connected to Steam."; break;
                    case 4:
                        e = "The connection timed out because the target user didn't respond, perhaps they aren't calling AcceptP2PSessionWithUser"; break;
                }

                if(e != null)
                {
                    Logging.BMSLog.Log("<color=yellow>" + e + "</color>");
                }
            }
        }
#endregion

        SteamAPICall_t m_CreateLobbyResult;

        public SteamP2PServer(int maxConnections) : base(maxConnections)
        {
            BannedAddresses = new List<string>();
            commonServerLogic = new CommonServerLogic(this);
            m_CallbackP2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            m_CallbackP2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
        }

		protected List<FrameStream> bufferedMessages = new List<FrameStream>();

		public List<string> BannedAddresses { get; set; }

		/// <summary>
		/// Used to determine if this server is currently accepting connections
		/// </summary>
		public bool AcceptingConnections { get; private set; }

		public void Send(NetworkingPlayer player, FrameStream frame, bool reliable = false)
		{
            SteamP2PPacketComposer composer = new SteamP2PPacketComposer(this, player, frame, reliable);

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
                for (int i = 0; i < Players.Count; i++)
				{
                   NetworkingPlayer player = Players[i];

                    if (!commonServerLogic.PlayerIsReceiver(player, frame, ProximityDistance, skipPlayer))
						continue;

					try
					{
						Send(player, frame, reliable);
					}
					catch(Exception e)
					{
                        Logging.BMSLog.LogException(e);
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
        public delegate void OnLobbyReady();
        public void Host(CSteamID selfSteamId, ELobbyType lobbyType, OnLobbyReady func = null)
		{
			if (Disposed)
				throw new ObjectDisposedException("SteamP2PServer", "This object has been disposed and can not be used to connect, please use a new SteamP2PServer");

			try
			{
				Client = new CachedSteamP2PClient(selfSteamId);
				Me = new NetworkingPlayer(ServerPlayerCounter++, selfSteamId, true, this);
				Me.InstanceGuid = InstanceGuid.ToString();

                m_CallbackLobbyCreated = Callback<LobbyCreated_t>.Create((LobbyCreated_t data) =>
                {
                    LobbyID = (CSteamID)data.m_ulSteamIDLobby;
                    if (func != null)
                        func();
                });

                m_CreateLobbyResult = SteamMatchmaking.CreateLobby(lobbyType, 5);
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
                StartAcceptingConnections();
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
            // Since we are disconnecting we need to stop the read thread
            Logging.BMSLog.Log("<color=cyan>SteamP2P server disconnecting...</color>");
            StopAcceptingConnections();
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

            if (LobbyID.IsLobby())
                SteamMatchmaking.LeaveLobby(LobbyID);
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
            Logging.BMSLog.LogFormat("Removing player {0}:{1} forced={2}", player.NetworkId, player.SteamID.m_SteamID, forced);

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
            Logging.BMSLog.LogFormat("SteamP2PServer:FinalizeRemovePlayer({0}:{1}, forced={2}", player.NetworkId, player.SteamID.m_SteamID, forced);
			steamPlayers.Remove(player.SteamID);
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
            CSteamID messageFrom = default(CSteamID);

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
                    uint msgSize = 0;

                    if (SteamNetworking.IsP2PPacketAvailable(out msgSize))
                    {
                        packet = Client.Receive(msgSize, out messageFrom);
                    }
                    else
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    if (PacketLossSimulation > 0.0f && new Random().NextDouble() <= PacketLossSimulation)
					{
						// Skip this message
						continue;
					}

					BandwidthIn += (ulong)packet.Size;
				}
				catch(Exception e)
				{
                    Logging.BMSLog.LogException(e);

					SteamNetworkingPlayer player;
					if (steamPlayers.TryGetValue(messageFrom, out player))
					{
						FinalizeRemovePlayer(player, true);
					}

					continue;
				}

				// Check to make sure a message was received
				if (packet == null || packet.Size <= 0)
					continue;

				if (!steamPlayers.ContainsKey(messageFrom))
				{
                    SetupClient(packet, messageFrom);
					continue;
				}
				else
				{
					currentReadingPlayer = steamPlayers[messageFrom];

					if (!currentReadingPlayer.Accepted && !currentReadingPlayer.PendingAccepted)
					{
						// It is possible that the response validation was dropped so
						// check if the client is resending for a response
						byte[] response = Websockets.ValidateConnectionHeader(packet.CompressBytes());

						// The client has sent the connection request again
						if (response != null)
						{
							Client.Send(response, response.Length, messageFrom, EP2PSend.k_EP2PSendReliable);
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
						// Due to the Forge Networking protocol, the only time that packet 1
						// will be 71 and the second packet be 69 is a forced disconnect reconnect
						if (packet[0] == 71 && packet[1] == 69)
						{
                            Logging.BMSLog.LogFormat("Received packet[0]=71 & packet[1]=69");
							steamPlayers.Remove(messageFrom);
							FinalizeRemovePlayer(currentReadingPlayer, true);
							continue;
						}

						currentReadingPlayer.Ping();
						ReadPacket(packet);
					}
				}
			}
		}

        private void SetupClient(BMSByte packet, CSteamID steamId)
        {
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

            SteamNetworkingPlayer player = new SteamNetworkingPlayer(ServerPlayerCounter++, steamId, false, this);

            // If all is in order then send the validated response to the client
            Client.Send(response, response.Length, steamId);

            OnPlayerConnected(player);
            steamPlayers.Add(steamId, player);

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
			catch (Exception e)
			{
				Logging.BMSLog.LogException(e);

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

            if (LobbyID.IsLobby())
                SteamMatchmaking.SetLobbyJoinable(LobbyID, false);
        }

		public void StartAcceptingConnections()
		{
			AcceptingConnections = true;
            if (LobbyID.IsLobby())
                SteamMatchmaking.SetLobbyJoinable(LobbyID, true);
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
#endif

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
using BeardedManStudios.Forge.Networking.Nat;
using BeardedManStudios.Threading;
using Steamworks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class SteamP2PClient : BaseSteamP2P, IClient
	{
		/// <summary>
		/// The max amount of tries that this client will attempt to connect to the server
		/// where there is 3 seconds between each attempt
		/// </summary>
		public const int CONNECT_TRIES = 10;

		/// <summary>
		/// The hash that is / was validated by the server
		/// </summary>
		private string headerHash = string.Empty;

		/// <summary>
		/// Used to determine if the client has requested to be accepted by the server
		/// </summary>
		private bool headerExchanged = false;

		/// <summary>
		/// The identity of the server as a player
		/// </summary>
		private NetworkingPlayer server = null;
		public NetworkingPlayer Server { get { return server; } }

		public UDPPacketManager packetManager = new UDPPacketManager();

		public event BaseNetworkEvent connectAttemptFailed;

        protected Callback<P2PSessionConnectFail_t> m_CallbackP2PSessionConnectFail;

        public override void Send(FrameStream frame, bool reliable = false)
		{
            /*//might have to go back to UDP packet composers. needs testing
            byte[] data = frame.GetData(reliable);

            Client.Send(data, data.Length, Server.SteamID, reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable);*/

            SteamP2PPacketComposer composer = new SteamP2PPacketComposer(this, Server, frame, reliable);

            // If this message is reliable then make sure to keep a reference to the composer
            // so that there are not any run-away threads
            if (reliable)
            {
                // Use the completed event to clean up the object from memory
                composer.completed += ComposerCompleted;
                pendingComposers.Add(composer);
            }

            //TODO: New constructor for setting up callbacks before regular constructor (as seen above)
            //composer.Init(this, Server, frame, reliable);
        }

		/// <summary>
		/// Sends binary message to the specified receiver(s)
		/// </summary>
		/// <param name="receivers">The clients / server to receive the message</param>
		/// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
		/// <param name="reliable">True if message must be delivered</param>
		/// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
		public virtual void Send(Receivers receivers = Receivers.Server, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, bool reliable = false , params object[] objectsToSend)
		{
			BMSByte data = ObjectMapper.BMSByte(objectsToSend);
			Binary sendFrame = new Binary(Time.Timestep, false, data, receivers, messageGroupId, false);
			Send(sendFrame, reliable);
		}
        private SteamAPICall_t m_JoinCall;
        private Callback<LobbyEnter_t> m_LobbyEntered;
		/// <summary>
		/// This will connect a UDP client to a given UDP server
		/// </summary>
		/// <param name="host">The server's host address on the network</param>
		/// <param name="port">The port that the server is hosting on</param>
		/// <param name="natHost">The NAT server host address, if blank NAT will be skipped</param>
		/// <param name="natPort">The port that the NAT server is hosting on</param>
		/// <param name="pendCreates">Immidiately set the NetWorker::PendCreates to true</param>
		public void Connect(CSteamID hostId, bool pendCreates = false)
		{
			if (Disposed)
				throw new ObjectDisposedException("UDPClient", "This object has been disposed and can not be used to connect, please use a new UDPClient");

            if(hostId.IsLobby())
            {
                //If this is a lobby we need to join it, make direct connection to the owner.
                m_JoinCall = SteamMatchmaking.JoinLobby(hostId);
                m_LobbyEntered = Callback<LobbyEnter_t>.Create((LobbyEnter_t data) =>
                {
                    LobbyID = (CSteamID)data.m_ulSteamIDLobby;
                    //Get the owner and attempt a direct connection
                    Connect(SteamMatchmaking.GetLobbyOwner(LobbyID), pendCreates);
                });
                return;
            }
			// By default pending creates should be true and flushed when ready
			if (!pendCreates)
				PendCreates = true;

			try
			{
				ushort clientPort = DEFAULT_PORT;

				// Make sure not to listen on the same port as the server for local networks
				if (clientPort == DEFAULT_PORT)
					clientPort++;

				Client = new CachedSteamP2PClient(hostId);

				// Do any generic initialization in result of the successful bind
				OnBindSuccessful();

				// Get a random hash key that needs to be used for validating that the server was connected to
				headerHash = Websockets.HeaderHashKey();

				// This is a typical Websockets accept header to be validated
				byte[] connectHeader = Websockets.ConnectionHeader(headerHash, DEFAULT_PORT);

				// Setup the identity of the server as a player
				server = new NetworkingPlayer(0, hostId, true, this);

				// Create the thread that will be listening for new data from connected clients and start its execution
				Task.Queue(ReadNetwork);

				//Let myself know I connected successfully
				OnPlayerConnected(server);

				// Set myself as a connected client
				server.Connected = true;

				//Set the port
				SetPort(clientPort);

				int connectCounter = 0;
				Task.Queue(() =>
				{
					do
					{
						// Send the accept headers to the server to validate
						Client.Send(connectHeader, connectHeader.Length, hostId, EP2PSend.k_EP2PSendReliable);
						Thread.Sleep(3000);
					} while (!headerExchanged && IsBound && ++connectCounter < CONNECT_TRIES);

					if (connectCounter >= CONNECT_TRIES)
					{
						if (connectAttemptFailed != null)
							connectAttemptFailed(this);
					}
				});

                m_CallbackP2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create((P2PSessionConnectFail_t data) =>
                {
                    if(data.m_eP2PSessionError > 0)
                    {
                        Disconnect(true);
                        switch (data.m_eP2PSessionError)
                        {
                            case 1:
                                Logging.BMSLog.LogException("The target user is not running the same game."); break;
                            case 2:
                                Logging.BMSLog.LogException("The local user doesn't own the app that is running."); break;
                            case 3:
                                Logging.BMSLog.LogException("Target user isn't connected to Steam."); break;
                            case 4:
                                Logging.BMSLog.LogException("The connection timed out because the target user didn't respond, perhaps they aren't calling AcceptP2PSessionWithUser"); break;
                        }
                    }
                    //
                });

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
		/// Disconnect this client from the server
		/// </summary>
		/// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
		public override void Disconnect(bool forced)
		{
            Logging.BMSLog.Log("<color=cyan>SteamP2P client disconnecting...</color>");

            if (LobbyID.IsLobby())
                SteamMatchmaking.LeaveLobby(LobbyID);

            if (Client == null)
				return;

			lock (Client)
			{
				if (forced)
					CloseConnection();
				else
				{
					var frame = new ConnectionClose(Time.Timestep, false, Receivers.Server, MessageGroupIds.DISCONNECT, false);
					Send(frame, true);
					Task.Queue(CloseConnection, 1000);
				}

				// Send signals to the methods registered to the disconnect events
				if (forced)
					//	OnDisconnected();
					//else
					OnForcedDisconnect();
			}
		}

		/// <summary>
		/// Infinite loop listening for new data from all connected clients on a separate thread.
		/// This loop breaks when readThreadCancel is set to true
		/// </summary>
		private void ReadNetwork()
		{
            CSteamID messageFrom = default(CSteamID);

			try
			{
				BMSByte packet = null;
				// Intentional infinite loop
				while (IsBound)
				{
					// If the read has been flagged to be canceled then break from this loop
					if (readThreadCancel)
						return;

					try
					{
                        uint msgSize = 0;

                        if(SteamNetworking.IsP2PPacketAvailable(out msgSize))
                        {
                            packet = Client.Receive(msgSize, out messageFrom);
                        }
                        else
                        {
                            Thread.Sleep(1);
                            continue;
                        }
						// Read a packet from the network


						if (PacketLossSimulation > 0.0f && new Random().NextDouble() <= PacketLossSimulation)
						{
							// Skip this message
							continue;
						}

						BandwidthIn += (ulong)packet.Size;
					}
					catch (SocketException ex)
					{
						// This is a common exception when we exit the blocking call
						Logging.BMSLog.LogException(ex);
						Disconnect(true);
					}

					// Check to make sure a message was received
					if (packet == null || packet.Size <= 0)
						continue;

					// Check to see if the headers have been exchanged
					if (!headerExchanged)
					{
						if (Websockets.ValidateResponseHeader(headerHash, packet.CompressBytes()))
						{
							headerExchanged = true;

							// TODO:  When getting the user id, it should also get the server time
							// by using the current time in the payload and getting it back along with server time

							// Ping the server to finalize the player's connection
							Send(Text.CreateFromString(Time.Timestep, InstanceGuid.ToString(), false, Receivers.Server, MessageGroupIds.NETWORK_ID_REQUEST, false), true);
						}
						else if (packet.Size != 1 || packet[0] != 0)
						{
                            Logging.BMSLog.LogWarning("DISCONNECTED: RECEIVED UNKNOWN PACKET BEFORE HEADERS WERE EXCHANGED!");
                            Disconnect(true);
							break;
						}
						else
							continue;
					}
					else
					{
						if (packet.Size < 17)
							continue;

						// Format the byte data into a UDPPacket struct
						UDPPacket formattedPacket = TranscodePacket(Server, packet);

						// Check to see if this is a confirmation packet, which is just
						// a packet to say that the reliable packet has been read
						if (formattedPacket.isConfirmation)
						{
							if (formattedPacket.groupId == MessageGroupIds.DISCONNECT)
							{
								CloseConnection();
								return;
							}

							OnMessageConfirmed(server, formattedPacket);
							continue;
						}

                        if (formattedPacket.groupId == MessageGroupIds.AUTHENTICATION_FAILURE)
                        {
                            Logging.BMSLog.LogWarning("The server rejected the authentication attempt");
                            // Wait for the second message (Disconnect)
                            continue;
                        }

						// Add the packet to the manager so that it can be tracked and executed on complete
						packetManager.AddPacket(formattedPacket, PacketSequenceComplete, this);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.BMSLog.LogException(ex);
				Disconnect(true);
			}
		}

		private void PacketSequenceComplete(BMSByte data, int groupId, byte receivers, bool isReliable)
		{
			// Pull the frame from the sent message
			FrameStream frame = Factory.DecodeMessage(data.CompressBytes(), false, groupId, Server, receivers);

			if (isReliable)
			{
				frame.ExtractReliableId();

				// TODO:  If the current reliable index for this player is not at
				// the specified index, then it needs to wait for the correct ordering
				Server.WaitReliable(frame);
			}
			else
				FireRead(frame, Server);
		}

		private void CloseConnection()
		{

			if (Client == null)
				return;

			OnDisconnected();

			// Close our CachedUDPClient so that it can no longer be used
			Client.Close();
			Client = null;
		}

		/// <summary>
		/// Request the ping from the server (pingReceived will be triggered if it receives it)
		/// This is not a reliable call
		/// </summary>
		public override void Ping()
		{
			Send(GeneratePing());
		}

		/// <summary>
		/// A ping was receieved from the server so we need to respond with the pong
		/// </summary>
		/// <param name="playerRequesting">The server</param>
		/// <param name="time">The time that the ping was received for</param>
		protected override void Pong(NetworkingPlayer playerRequesting, DateTime time)
		{
			Send(GeneratePong(time));
		}

		public override void FireRead(FrameStream frame, NetworkingPlayer currentPlayer)
		{
			if (frame is ConnectionClose)
			{
				CloseConnection();
				return;
			}

            if (frame.GroupId == MessageGroupIds.AUTHENTICATION_CHALLENGE)
            {
                if ((Me != null && Me.Connected) || authenticator == null)
                    return;

                authenticator.AcceptChallenge(this, frame.StreamData, AuthServer, RejectServer);

                return;
            }

            // Send an event off that a packet has been read
            OnMessageReceived(currentPlayer, frame);
		}

        /// <summary>
        /// Callback for user auth. Sends an authentication response to the server.
        /// </summary>
        private void AuthServer(BMSByte buffer)
        {
            Send(new Binary(Time.Timestep, false, buffer, Receivers.Server, MessageGroupIds.AUTHENTICATION_RESPONSE, false), true);
        }

        /// <summary>
        /// Callback for user auth. Disconnects the user from an invalid server.
        /// </summary>
        private void RejectServer()
        {
            Disconnect(true);
        }
    }
}
#endif

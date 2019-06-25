﻿#if FACEPUNCH_STEAMWORKS
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Threading;
using Steamworks;
using System;
using System.Net.Sockets;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class FacepunchP2PClient : BaseFacepunchP2P, IClient
	{
		/// <summary>
		/// The max amount of tries that this client will attempt to connect to the server
		/// where there is 3 seconds between each attempt
		/// </summary>
		public const int CONNECT_TRIES = 10;
		public UDPPacketManager packetManager = new UDPPacketManager();
		public event BaseNetworkEvent connectAttemptFailed;

		/// <summary>
		/// The identity of the server as a player
		/// </summary>
		private NetworkingPlayer server = null;
		public NetworkingPlayer Server { get { return server; } }

		/// <summary>
		/// The hash that is / was validated by the server
		/// </summary>
		private string headerHash = string.Empty;

		/// <summary>
		/// Used to determine if the client has requested to be accepted by the server
		/// </summary>
		private bool headerExchanged = false;

		/// <summary>
		/// Cached default steamId to check against
		/// </summary>
		private readonly SteamId defaultSteamId = default(SteamId);

		/// <summary>
		/// Sends data to the server
		/// </summary>
		/// <param name="frame">Data to send</param>
		/// <param name="reliable">Send reliable (slow) or unreliable (fast)</param>
		public override void Send(FrameStream frame, bool reliable = false)
		{
			var composer = new FacepunchP2PPacketComposer(this, Server, frame, reliable);

			// If this message is reliable then make sure to keep a reference to the composer
			// so that there are not any run-away threads
			if (reliable)
			{
				// Use the completed event to clean up the object from memory
				composer.completed += ComposerCompleted;
				pendingComposers.Add(composer);
			}
		}

		/// <summary>
		/// Sends binary message to the specified receiver(s)
		/// </summary>
		/// <param name="receivers">The clients / server to receive the message</param>
		/// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
		/// <param name="reliable">True if message must be delivered</param>
		/// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
		public virtual void Send(Receivers receivers = Receivers.Server, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, bool reliable = false, params object[] objectsToSend)
		{
			BMSByte data = ObjectMapper.BMSByte(objectsToSend);
			Binary sendFrame = new Binary(Time.Timestep, false, data, receivers, messageGroupId, false);
			Send(sendFrame, reliable);
		}

		/// <summary>
		/// Connect this FacepunchP2PClient to a steam lobby
		/// </summary>
		/// <param name="lobbyToJoin">The Facepunch <see cref="Steamworks.Data.Lobby"/> object to join</param>
		/// <param name="pendCreates">Immediately set the NetWorker::PendCreates to true</param>
		public void Connect(Steamworks.Data.Lobby lobbyToJoin, bool pendCreates = false)
		{
			if (Disposed)
				throw new ObjectDisposedException("FacepunchP2PClient", "This object has been disposed and can not be used to connect, please use a new FacepunchP2PClient");

			ConnectToLobbyAsync(lobbyToJoin, pendCreates);
		}

		/// <summary>
		/// Connect this FacepunchP2PClient directly to a steam user's FacepunchP2PServer with SteamId specified
		/// </summary>
		/// <param name="hostId">The host's <see cref="SteamId"/> SteamId object</param>
		/// <param name="pendCreates">Immediately set the NetWorker::PendCreates to true</param>
		public void Connect(SteamId hostId, bool pendCreates = false)
		{
			if (Disposed)
				throw new ObjectDisposedException("FacepunchP2PClient", "This object has been disposed and can not be used to connect, please use a new FacepunchP2PClient");

			// By default pending creates should be true and flushed when ready
			if (!pendCreates)
				PendCreates = true;

			try
			{
				ushort clientPort = DEFAULT_PORT;

				// Make sure not to listen on the same port as the server for local networks
				if (clientPort == DEFAULT_PORT)
					clientPort++;

				Client = new CachedFacepunchP2PClient(hostId);

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
						Client.Send(connectHeader, connectHeader.Length, hostId, P2PSend.Reliable);
						Thread.Sleep(3000);
					} while (!headerExchanged && IsBound && ++connectCounter < CONNECT_TRIES);

					if (connectCounter >= CONNECT_TRIES)
					{
						if (connectAttemptFailed != null)
							connectAttemptFailed(this);
					}
				});
			}
			catch (Exception e)
			{
				Logging.BMSLog.LogException(e);
				// Do any generic initialization in result of the binding failure
				OnBindFailure();

				throw new FailedBindingException("Failed to bind to server's SteamId, see inner exception", e);
			}
		}

		/// <summary>
		/// Connects to a Steam Lobby then kicks off connection to the lobby owner's SteamId
		/// </summary>
		/// <param name="lobbyToJoin">The <see cref="Steamworks.Data.Lobby"/> to join</param>
		/// <param name="pendCreates">Set the NetWorker::PendCreates to true</param>
		private async void ConnectToLobbyAsync(Steamworks.Data.Lobby lobbyToJoin, bool pendCreates)
		{
			RoomEnter roomEnter = await lobbyToJoin.Join();
			if (roomEnter != RoomEnter.Success)
				return;
			Lobby = lobbyToJoin;
			Connect(Lobby.Owner.Id, pendCreates);
		}

		/// <summary>
		/// Disconnect this client from the server
		/// </summary>
		/// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
		public override void Disconnect(bool forced)
		{
			Logging.BMSLog.Log("<color=cyan>FacepunchP2P client disconnecting...</color>");

			if (Lobby.Id.Value != 0)
				Lobby.Leave();

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
					OnForcedDisconnect();
			}
		}

		/// <summary>
		/// Infinite loop listening for new data from all connected clients on a separate thread.
		/// This loop breaks when readThreadCancel is set to true
		/// </summary>
		private void ReadNetwork()
		{
			var messageFrom = defaultSteamId;

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
						if (SteamNetworking.IsP2PPacketAvailable())
						{
							packet = Client.Receive(out messageFrom);
						}
						else
						{
							Thread.Sleep(1);
							continue;
						}

						if (messageFrom == default(SteamId))
						{
							Thread.Sleep(1);
							continue;
						}

						if (packet == null)
						{
							Logging.BMSLog.LogWarning("Null packet received from player: " + messageFrom.Value);
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

		/// <summary>
		/// Parse the packet into a byte [] after it has been accepted and added to the queue for reading
		/// If reliable, packets must be ordered before reading
		/// </summary>
		/// <param name="data">Raw data in the packet</param>
		/// <param name="groupId"></param>
		/// <param name="receivers"></param>
		/// <param name="isReliable"></param>
		private void PacketSequenceComplete(BMSByte data, int groupId, byte receivers, bool isReliable)
		{
			// Pull the frame from the sent message
			var frame = Factory.DecodeMessage(data.CompressBytes(), false, groupId, Server, receivers);

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

		/// <summary>
		/// Disconnect and destroy the CachedFacepunchP2PClient
		/// </summary>
		private void CloseConnection()
		{
			if (Client == null)
				return;

			OnDisconnected();

			// Close our CachedFacepunchP2PClient so that it can no longer be used
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

		/// <summary>
		/// Reads the frame stream as if it were read on the network
		/// </summary>
		/// <param name="frame">Data extracted from BMSByte packet</param>
		/// <param name="currentPlayer">Client who sent the data</param>
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

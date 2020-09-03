using System;
using System.Net;
using System.Net.Sockets;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Nat;
using BeardedManStudios.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class UDPClient : BaseUDP, IClient
	{
		/// <summary>
		/// The max amount of tries that this client will attempt to connect to the server
		/// where there is 3 seconds between each attempt
		/// </summary>
		public const int CONNECT_TRIES = 10;

		/// <summary>
		/// The minimum size of a frame
		/// </summary>
		private const int MINIMUM_FRAME_SIZE = 17;

		/// <summary>
		/// The hash that is / was validated by the server
		/// </summary>
		private string headerHash = string.Empty;

		/// <summary>
		/// Used to determine if the client has requested to be accepted by the server
		/// </summary>
		private bool initialConnectHeaderExchanged = false;

		private bool IsSimulatedPacketLoss => PacketLossSimulation > 0.0f && new Random().NextDouble() <= PacketLossSimulation;

		/// <summary>
		/// The identity of the server as a player
		/// </summary>
		private NetworkingPlayer server = null;

		[Obsolete("Use ServerPlayer instead, it has a much more clear intent")]
		public NetworkingPlayer Server { get { return ServerPlayer; } }
		public NetworkingPlayer ServerPlayer { get { return server; } }

		public UDPPacketManager packetManager = new UDPPacketManager();

		public NatHolePunch nat = new NatHolePunch();

		public event BaseNetworkEvent connectAttemptFailed;

		public override void Send(FrameStream frame, bool reliable = false)
		{
			UDPPacketComposer composer = new UDPPacketComposer();

			// If this message is reliable then make sure to keep a reference to the composer
			// so that there are not any run-away threads
			if (reliable)
			{
				composer.completed += ComposerCompleted;
				pendingComposers.Add(composer);
			}

			composer.Init(this, ServerPlayer, frame, reliable);
		}

		/// <summary>
		/// Sends binary message to the specified receiver(s)
		/// </summary>
		/// <param name="receivers">The clients / server to receive the message</param>
		/// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
		/// <param name="reliable">True if message must be delivered</param>
		/// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
		public virtual void Send(Receivers receivers = Receivers.Server,
			int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS,
			bool reliable = false, params object[] objectsToSend)
		{
			BMSByte data = ObjectMapper.BMSByte(objectsToSend);
			Binary sendFrame = new Binary(Time.Timestep, false, data, receivers, messageGroupId, false);
			Send(sendFrame, reliable);
		}

		private ushort FindAvailablePort(ushort clientPort, ushort port)
		{
			if (clientPort == port)
				clientPort++;

			for (; ; clientPort++)
			{
				try
				{
					Client = new CachedUdpClient(clientPort);
					break;
				}
				catch
				{
					if (port == 0)
						throw new BaseNetworkException("There were no ports available starting from port " + port);
				}
			}

			return clientPort;
		}

		private void AttemptServerConnection()
		{
			int connectCounter = 0;

			// This is a typical Websockets accept header to be validated
			byte[] connectHeader = Websockets.ConnectionHeader(headerHash, Port);

			do
			{
				// Send the accept headers to the server to validate
				Client.Send(connectHeader, connectHeader.Length, ServerPlayer.IPEndPointHandle);
				Task.Sleep(3000);
			} while (!initialConnectHeaderExchanged && IsActiveSession && ++connectCounter < CONNECT_TRIES);

			if (connectCounter >= CONNECT_TRIES && connectAttemptFailed != null)
				connectAttemptFailed(this);
		}

		private void SetupConnectingState()
		{
			// Create the thread that will be listening for new data from connected clients and start its execution
			Task.Queue(ReadNetwork);

			// Let myself know I connected successfully
			OnPlayerConnected(server);

			// Set myself as a connected client
			server.Connected = true;
		}

		private void SetNetworkBindings(ushort overrideBindingPort, ushort port, string natHost, string host, ushort natPort)
		{
			// Make sure not to listen on the same port as the server for local networks
			ushort clientPort = FindAvailablePort(overrideBindingPort, port);
			Client.EnableBroadcast = true;

			// If the server is behind a NAT, request for the port punch by the nat server
			if (!string.IsNullOrEmpty(natHost))
				nat.Connect(host, port, clientPort, natHost, natPort);

			// Do any generic initialization in result of the successful bind
			OnBindSuccessful();

			// Get a random hash key that needs to be used for validating that the server was connected to
			headerHash = Websockets.HeaderHashKey();

			// Set the port
			SetPort(clientPort);
		}

		private void CreateTheNetworkingPlayer(string host, ushort port)
		{
			try
			{
				// Setup the identity of the server as a player
				server = new NetworkingPlayer(0, host, true, HostResolver.Resolve(host, port), this);
			}
			catch (ArgumentException)
			{
				if (connectAttemptFailed != null)
					connectAttemptFailed(this);

				throw;
			}
		}

		private void BindAndConnect(ushort overrideBindingPort, ushort port, string natHost, string host, ushort natPort)
		{
			SetNetworkBindings(overrideBindingPort, port, natHost, host, natPort);
			CreateTheNetworkingPlayer(host, port);
			SetupConnectingState();
			Task.Queue(AttemptServerConnection);
		}

		/// <summary>
		/// This will connect a UDP client to a given UDP server
		/// </summary>
		/// <param name="host">The server's host address on the network</param>
		/// <param name="port">The port that the server is hosting on</param>
		/// <param name="natHost">The NAT server host address, if blank NAT will be skipped</param>
		/// <param name="natPort">The port that the NAT server is hosting on</param>
		/// <param name="pendCreates">Immidiately set the NetWorker::PendCreates to true</param>
		public void Connect(string host, ushort port = DEFAULT_PORT, string natHost = "",
			ushort natPort = NatHolePunch.DEFAULT_NAT_SERVER_PORT, bool pendCreates = false,
			ushort overrideBindingPort = DEFAULT_PORT + 1)
		{
			if (Disposed)
			{
				throw new ObjectDisposedException("UDPClient",
					"This object has been disposed and can not be used to connect, please use a new UDPClient");
			}

			// By default pending creates should be true and flushed when ready
			if (!pendCreates)
				PendCreates = true;

			try
			{
				BindAndConnect(overrideBindingPort, port, natHost, host, natPort);
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
			try
			{
				while (IsActiveSession)
				{
					// If the read has been flagged to be canceled then break from this loop
					if (IsReadThreadCancelPending)
						return;

					ReceiveNetworkData();
				}
			}
			catch (Exception ex)
			{
				Logging.BMSLog.LogException(ex);
				Disconnect(true);
			}
		}

		private bool IsNotFromServer(IPEndPoint groupEP)
		{
			return groupEP.Address != ServerPlayer.IPEndPointHandle.Address
				&& groupEP.Port != ServerPlayer.IPEndPointHandle.Port;
		}

		private bool IsBroadcastedListingRequest(BMSByte packet)
		{
			return packet.Size.Between(2, 4)
				&& packet[0] == BROADCAST_LISTING_REQUEST_1
				&& packet[1] == BROADCAST_LISTING_REQUEST_2
				&& packet[2] == BROADCAST_LISTING_REQUEST_3;
		}

		private void CompleteHeaderExchange()
		{
			initialConnectHeaderExchanged = true;

			// TODO:  When getting the user id, it should also get the server time by using
			// the current time in the payload and getting it back along with server time

			// Ping the server to finalize the player's connection
			var textFrame = Text.CreateFromString(Time.Timestep, InstanceGuid.ToString(), false,
				Receivers.Server, MessageGroupIds.NETWORK_ID_REQUEST, false);

			Send(textFrame, true);
		}

		/// <summary>
		/// The server sent us a message before sending a response header to validate
		/// This happens if the server is not accepting connections or the max connection count has been reached
		/// We will get two messages. The first one is either a MAX_CONNECTIONS or NOT_ACCEPT_CONNECTIONS group message.
		/// The second one will be the DISCONNECT message
		/// </summary>
		/// <param name="packet">The packet recieved causing relating to the rejection</param>
		private void HandleServerRejection(BMSByte packet)
		{
			UDPPacket formattedPacket = TranscodePacket(ServerPlayer, packet);

			if (formattedPacket.groupId == MessageGroupIds.MAX_CONNECTIONS)
				Logging.BMSLog.LogWarning("Max Players Reached On Server");
			else if (formattedPacket.groupId == MessageGroupIds.NOT_ACCEPT_CONNECTIONS)
				Logging.BMSLog.LogWarning("The server is busy and not accepting connections");
			else if (formattedPacket.groupId == MessageGroupIds.DISCONNECT)
				CloseConnection();
			else
			{
				// Received something unexpected so do the same thing as the if below
				Disconnect(true);
				CancelReadThread();
			}
		}

		private void HandleDisconnectableGroupId(UDPPacket formattedPacket)
		{
			// Check to see if this is a confirmation packet, which is just
			// a packet to say that the reliable packet has been read
			if (formattedPacket.isConfirmation)
			{
				if (formattedPacket.groupId != MessageGroupIds.DISCONNECT)
					OnMessageConfirmed(server, formattedPacket);
			}
			else if (formattedPacket.groupId == MessageGroupIds.AUTHENTICATION_FAILURE)
				Logging.BMSLog.LogWarning("The server rejected the authentication attempt");

			CancelReadThread();
		}

		private void HandleHeaderExchanging(BMSByte packet)
		{
			if (Websockets.ValidateResponseHeader(headerHash, packet.CompressBytes()))
			{
				CompleteHeaderExchange();
			}
			else if (packet.Size >= MINIMUM_FRAME_SIZE)
			{
				HandleServerRejection(packet);
				CancelReadThread();
			}
			else if (packet.Size != 1 || packet[0] != 0)
			{
				Disconnect(true);
				CancelReadThread();
			}
		}

		private void HandlePacket(BMSByte packet)
		{
			if (packet.Size < MINIMUM_FRAME_SIZE)
			{
				CancelReadThread();
				return;
			}

			// Format the byte data into a UDPPacket struct
			UDPPacket formattedPacket = TranscodePacket(ServerPlayer, packet);

			if (IsDisconnectableGroupId(formattedPacket))
				HandleDisconnectableGroupId(formattedPacket);
			else if (!formattedPacket.isConfirmation)
				packetManager.AddAndTrackPacket(formattedPacket, PacketSequenceComplete, this);
		}

		private void HandlePacketNotFromServer(BMSByte packet, IPEndPoint groupEP)
		{
			// This may be a local listing request so respond with the client flag byte
			if (IsBroadcastedListingRequest(packet))
				Client.Send(new byte[] { CLIENT_BROADCAST_CODE }, 1, groupEP);
		}

		private void ProcessRawPacket(BMSByte packet, IPEndPoint groupEP)
		{
			if (IsSimulatedPacketLoss || packet == null || packet.Size <= 0)
				return;

			if (IsNotFromServer(groupEP))
				HandlePacketNotFromServer(packet, groupEP);
			else if (!initialConnectHeaderExchanged)
				HandleHeaderExchanging(packet);
			else
				HandlePacket(packet);
		}

		private void ReceiveNetworkData()
		{
			try
			{
				ReadPacket();
			}
			catch (SocketException /*ex*/)
			{
				// This is a common exception when we exit the blocking call
				//Logging.BMSLog.LogException(ex);
				Disconnect(true);
				CancelReadThread();
			}
		}

		private void ReadPacket()
		{
			BMSByte packet;
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);
			packet = Client.Receive(ref groupEP);
			BandwidthIn += (ulong)packet.Size;

			ProcessRawPacket(packet, groupEP);
		}

		private void ProcessCompletedReliableFrame(FrameStream frame)
		{
			frame.ExtractReliableId();

			// TODO:  If the current reliable index for this player is not at
			// the specified index, then it needs to wait for the correct ordering
			ServerPlayer.WaitReliable(frame);
		}

		private void PacketSequenceComplete(BMSByte data, int groupId, byte receivers, bool isReliable)
		{
			// Pull the frame from the sent message
			FrameStream frame = Factory.DecodeMessage(data.CompressBytes(), false, groupId, ServerPlayer, receivers);

			if (isReliable)
				ProcessCompletedReliableFrame(frame);
			else
				FireRead(frame, ServerPlayer);
		}

		private void CloseConnection()
		{
			nat.Disconnect();

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
		/// A ping was received from the server so we need to respond with the pong
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
				if (authenticator != null && (Me == null || !Me.Connected))
					authenticator.AcceptChallenge(this, frame.StreamData, AuthServer, RejectServer);
			}
			else
			{
				// Send an event off that a packet has been read
				OnMessageReceived(currentPlayer, frame);
			}
		}

		/// <summary>
		/// Callback for user auth. Sends an authentication response to the server.
		/// </summary>
		private void AuthServer(BMSByte buffer)
		{
			var binaryFrame = new Binary(Time.Timestep, false, buffer, Receivers.Server, MessageGroupIds.AUTHENTICATION_RESPONSE, false);
			Send(binaryFrame, true);
		}

		/// <summary>
		/// Callback for user auth. Disconnects the user from an invalid server.
		/// </summary>
		private void RejectServer()
		{
			Disconnect(true);
		}

		private bool IsDisconnectableGroupId(UDPPacket formattedPacket)
		{
			return formattedPacket.groupId == MessageGroupIds.DISCONNECT
				|| formattedPacket.groupId == MessageGroupIds.AUTHENTICATION_FAILURE;
		}
	}
}

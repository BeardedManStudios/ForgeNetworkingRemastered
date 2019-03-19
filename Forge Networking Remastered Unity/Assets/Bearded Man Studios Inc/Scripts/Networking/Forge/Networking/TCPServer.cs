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
using BeardedManStudios.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

#if WINDOWS_UWP
using Windows.Networking.Sockets;
using System.IO;
#else
using System.Net.Sockets;
#endif

namespace BeardedManStudios.Forge.Networking
{
    // new TCPServer as of September 2018
    /// <summary>
    /// This is the main TCP server object responsible for listening for incomming connections
    /// and reading any data sent from clients who are currently connected
    /// </summary>
    public class TCPServer : BaseTCP, IServer
    {
        private CommonServerLogic commonServerLogic;

        #region Delegates
        /// <summary>
        /// A delegate for handling any raw TcpClient events
        /// </summary>
#if WINDOWS_UWP
		public delegate void RawTcpClientEvent(StreamSocket client);
#else
        public delegate void RawTcpClientEvent(TcpClient client);
#endif
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a raw TcpClient has successfully connected
        /// </summary>
        public event RawTcpClientEvent rawClientConnected;

        /// <summary>
        /// Occurs when raw TcpClient has successfully connected
        /// </summary>
        public event RawTcpClientEvent rawClientDisconnected;

        /// <summary>
        /// Occurs when raw TcpClient has been forcibly closed
        /// </summary>
        public event RawTcpClientEvent rawClientForceClosed;
        #endregion

        /// <summary>
        /// The main listener for client connections
        /// </summary>
#if WINDOWS_UWP
		private StreamSocketListener listener = null;
#else
        private TcpListener listener = null;
#endif

        /// <summary>
        /// The ip address that is being bound to
        /// </summary>
        private IPAddress ipAddress = null;

        /// <summary>
        /// The main thread that will continuiously listen for new client connections
        /// </summary>
        //private Thread connectionThread = null;

        /// <summary>
        /// The raw list of all of the clients that are connected
        /// </summary>
#if WINDOWS_UWP
		private List<StreamSocket> rawClients = new List<StreamSocket>();
#else
        private List<TcpClient> rawClients = new List<TcpClient>();
#endif

        protected List<FrameStream> bufferedMessages = new List<FrameStream>();

        /// <summary>
        /// Used to determine if this server is currently accepting connections
        /// </summary>
        public bool AcceptingConnections { get; private set; }

        public List<string> BannedAddresses { get; set; }

        private readonly BufferManager bufferManager;

        public TCPServer(int maxConnections) : base(maxConnections)
        {
            AcceptingConnections = true;
            BannedAddresses = new List<string>();
            commonServerLogic = new CommonServerLogic(this);
            if (maxConnections > 128)
                bufferManager = new BufferManager(8192, 128, (maxConnections / 128) + 1);
            else
                bufferManager = new BufferManager(8192, maxConnections, 2);
        }

#if WINDOWS_UWP
		private void RawWrite(StreamSocket client, byte[] data)
#else
        private void RawWrite(TcpClient client, byte[] data)
#endif
        {
#if WINDOWS_UWP
			//Write data to the echo server.
			Stream streamOut = client.OutputStream.AsStreamForWrite();
			StreamWriter writer = new StreamWriter(streamOut);
			writer.Write(data);
			writer.Flush();
#else
            client.GetStream().Write(data, 0, data.Length);
#endif
        }

        /// <summary>
        /// The direct byte send method to the specified client
        /// </summary>
        /// <param name="client">The target client that will receive the frame</param>
        /// <param name="frame">The frame that is to be sent to the specified client</param>
#if WINDOWS_UWP
		public bool Send(StreamSocket client, FrameStream frame)
#else
        public bool Send(TcpClient client, FrameStream frame)
#endif
        {
            if (client == null)
                return false;

            // Make sure that we don't have any race conditions with writing to the same client
            lock (client)
            {
                try
                {
                    // Get the raw bytes from the frame and send them
                    byte[] data = frame.GetData();

                    RawWrite(client, data);
                    return true;
                }
                catch
                {
                    // The client is no longer connected or is unresponsive
                }
            }

            return false;
        }

        /// <summary>
        /// Sends binary message to the specific tcp client(s)
        /// </summary>
        /// <param name="client">The client to receive the message</param>
        /// <param name="receivers">Receiver's type</param>
        /// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
        /// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
        public bool Send(TcpClient client, Receivers receivers = Receivers.Target, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, params object[] objectsToSend)
        {
            BMSByte data = ObjectMapper.BMSByte(objectsToSend);
            Binary sendFrame = new Binary(Time.Timestep, false, data, Receivers.Target, messageGroupId, false);
#if WINDOWS_UWP
			public bool Send(StreamSocket client, FrameStream frame)
#else
            return Send(client, sendFrame);
#endif
        }

        /// <summary>
        /// Send a frame to a specific player
        /// </summary>
        /// <param name="frame">The frame to send to that specific player</param>
        /// <param name="targetPlayer">The specific player to receive the frame</param>
        public void SendToPlayer(FrameStream frame, NetworkingPlayer targetPlayer)
        {
            if (frame.Receivers == Receivers.AllBuffered || frame.Receivers == Receivers.OthersBuffered)
                bufferedMessages.Add(frame);

            lock (Players)
            {
                if (Players.Contains(targetPlayer))
                {
                    NetworkingPlayer player = Players[Players.IndexOf(targetPlayer)];
                    if (!player.Accepted && !player.PendingAccepted)
                        return;

                    if (player == frame.Sender)
                    {
                        // Don't send a message to the sending player if it was meant for others
                        if (frame.Receivers == Receivers.Others || frame.Receivers == Receivers.OthersBuffered || frame.Receivers == Receivers.OthersProximity || frame.Receivers == Receivers.OthersProximityGrid)
                            return;
                    }

                    // Check to see if the request is based on proximity
                    if (frame.Receivers == Receivers.AllProximity || frame.Receivers == Receivers.OthersProximity)
                    {
                        // If the target player is not in the same proximity zone as the sender
                        // then it should not be sent to that player
                        if (player.ProximityLocation.DistanceSquared(frame.Sender.ProximityLocation) > ProximityDistance * ProximityDistance)
                        {
                            return;
                        }
                    }

                    if (frame.Receivers == Receivers.AllProximityGrid || frame.Receivers == Receivers.OthersProximityGrid)
                    {
                        // If the target player is not in the same proximity zone as the sender
                        // then it should not be sent to that player
                        if (player.gridPosition.IsSameOrNeighbourCell(frame.Sender.gridPosition))
                        {
                            return;
                        }
                    }

                    try
                    {
                        Send(player.TcpClientHandle, frame);
                    }
                    catch
                    {
                        Disconnect(player, true);
                    }
                }
            }
        }

        /// <summary>
        /// Sends binary message to the specific client
        /// </summary>
        /// <param name="receivers">The clients / server to receive the message</param>
        /// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
        /// <param name="targetPlayer">The client to receive the message</param>
        /// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
        public void SendToPlayer(NetworkingPlayer targetPlayer, Receivers receivers = Receivers.Target, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, params object[] objectsToSend)
        {
            BMSByte data = ObjectMapper.BMSByte(objectsToSend);
            Binary sendFrame = new Binary(Time.Timestep, false, data, receivers, messageGroupId, false);
            SendToPlayer(sendFrame, targetPlayer);
        }

        /// <summary>
        /// Goes through all of the currently connected players and send them the frame
        /// </summary>
        /// <param name="frame">The frame to send to all of the connected players</param>
        public void SendAll(FrameStream frame, NetworkingPlayer skipPlayer = null)
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
                        Send(player.TcpClientHandle, frame);
                    }
                    catch
                    {
                        Disconnect(player, true);
                    }
                }
            }
        }

        // overload for ncw field distance check case
        public void SendAll(FrameStream frame, NetworkingPlayer sender, NetworkingPlayer skipPlayer = null)
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
                        Send(player.TcpClientHandle, frame);
                    } catch
                    {
                        Disconnect(player, true);
                    }
                }
            }
        }

        /// <summary>
        /// Goes through all of the currently connected players and send them the frame
        /// </summary>
        /// <param name="receivers">The clients / server to receive the message</param>
        /// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
        /// <param name="playerToIgnore">The client to ignore</param>
        /// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
        public void SendAll(Receivers receivers = Receivers.All, int messageGroupId = MessageGroupIds.START_OF_GENERIC_IDS, NetworkingPlayer playerToIgnore = null, params object[] objectsToSend)
        {
            BMSByte data = ObjectMapper.BMSByte(objectsToSend);
            Binary sendFrame = new Binary(Time.Timestep, false, data, receivers, messageGroupId, true);
            SendAll(sendFrame, playerToIgnore);
        }

        /// <summary>
        /// Call the base disconnect pending method to remove all pending disconnecting clients
        /// </summary>
        private void CleanupDisconnections() { DisconnectPending(RemovePlayer); }

        private bool PendingCommitDisconnects = false;

        /// <summary>
        /// Commits the disconnects
        /// </summary>
        public void CommitDisconnects() { CleanupDisconnections(); }

        /// <summary>
        /// This will begin the connection for TCP, this is a thread blocking operation until the connection
        /// is either established or has failed
        /// </summary>
        /// <param name="hostAddress">[127.0.0.1] Ip Address to host from</param>
        /// <param name="port">[15937] Port to allow connections from</param>
        public void Connect(string hostAddress = "0.0.0.0", ushort port = DEFAULT_PORT)
        {
            if (Disposed)
                throw new ObjectDisposedException("TCPServer", "This object has been disposed and can not be used to connect, please use a new ETCPServer");

            if (string.IsNullOrEmpty(hostAddress))
                throw new BaseNetworkException("An ip address must be specified to bind to. If you are unsure, you can set to 127.0.0.1");

            // Check to see if this server is being bound to a "loopback" address, if so then bind to any, otherwise bind to specified address
            if (hostAddress == "0.0.0.0" || hostAddress == "localhost")
                ipAddress = IPAddress.Any;
            else
                ipAddress = IPAddress.Parse(hostAddress);

            try
            {
                // Setup and start the base C# TcpListner
                listener = new TcpListener(ipAddress, port);
                //listener.Start();

                Me = new NetworkingPlayer(ServerPlayerCounter++, "0.0.0.0", true, listener, this);
                Me.InstanceGuid = InstanceGuid.ToString();

                // Create the thread that will be listening for clients and start its execution
                //Thread connectionThread = new Thread(new ThreadStart(ListenForConnections));
                //connectionThread.Start();
                //Task.Queue(ListenForConnections);
                listener.Start();
                listener.BeginAcceptTcpClient(ListenForConnections, listener);

                // Do any generic initialization in result of the successful bind
                OnBindSuccessful();

                // Create the thread that will be listening for new data from connected clients and start its execution
                //TODO: Task.Queue(ReadClients);

                // Remove client timeouts since TCP knows when the connection has been closed.
                //// Create the thread that will check for player timeouts
                //Task.Queue(() =>
                //{
                //    commonServerLogic.CheckClientTimeout((player) =>
                //    {
                //        Disconnect(player, true);
                //        OnPlayerTimeout(player);
                //        CleanupDisconnections();
                //    });
                //});

                //Let myself know I connected successfully
                OnPlayerConnected(Me);
                // Set myself as a connected client
                Me.Connected = true;

                //Set the port
                SetPort((ushort)((IPEndPoint)listener.LocalEndpoint).Port);
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
        /// Infinite loop listening for client connections on a separate thread.
        /// This loop breaks if there is an exception thrown on the blocking accept call
        /// </summary>
        private void ListenForConnections(IAsyncResult obj)
        {
            TcpListener asyncListener = (TcpListener)obj.AsyncState;
            TcpClient client = null;

            try
            {
                client = asyncListener.EndAcceptTcpClient(obj);
            }
            catch
            {
                return;
            }

            asyncListener.BeginAcceptTcpClient(ListenForConnections, asyncListener);

            if (rawClients.Count == MaxConnections)
            {
                // Tell the client why they are being disconnected
                Send(client, Error.CreateErrorMessage(Time.Timestep, "Max Players Reached On Server", false, MessageGroupIds.MAX_CONNECTIONS, true));

                // Send the close connection frame to the client
                Send(client, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, true));

                // Do disconnect logic for client
                ClientRejected(client, false);
                return;
            }
            else if (!AcceptingConnections)
            {
                // Tell the client why they are being disconnected
                Send(client, Error.CreateErrorMessage(Time.Timestep, "The server is busy and not accepting connections", false, MessageGroupIds.MAX_CONNECTIONS, true));

                // Send the close connection frame to the client
                Send(client, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, true));

                // Do disconnect logic for client
                ClientRejected(client, false);
                return;
            }

            ArraySegment<byte> segment;
            if (!bufferManager.TryTakeBuffer(out segment))
            {
                // Tell the client why they are being disconnected
                Send(client, Error.CreateErrorMessage(Time.Timestep, "The server is busy and not accepting connections", false, MessageGroupIds.MAX_CONNECTIONS, true));

                // Send the close connection frame to the client
                Send(client, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, true));

                // Do disconnect logic for client
                ClientRejected(client, false);
                throw new OutOfMemoryException("Buffer manager has run out of allocated memory (possible memory leak).");
            }

            // Clients will be looped through on other threads so be sure to lock it before adding
            ReceiveToken token;
            lock (Players)
            {
                rawClients.Add(client);

                // Create the identity wrapper for this player
                NetworkingPlayer player = new NetworkingPlayer(ServerPlayerCounter++, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), false, client, this);

                // Generically add the player and fire off the events attached to player joining
                OnPlayerConnected(player);

                token = new ReceiveToken
                {
                    internalBuffer = segment,
                    player = player,
                    bytesReceived = 0,
                    dataHolder = null,
                    maxAllowedBytes = 8192
                };
            }

            // Let all of the event listeners know that the client has successfully connected
            if (rawClientConnected != null)
                rawClientConnected(client);

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveAsync_Completed);
            e.UserToken = token;
            e.SetBuffer(token.internalBuffer.Array, token.internalBuffer.Offset, token.internalBuffer.Count);

            if (!client.Client.ReceiveAsync(e))
                Task.Queue(() => ReceiveAsync_Completed(this, e));


        }

        private void DoRead(SocketAsyncEventArgs e)
        {
            if (!IsBound)
            {
                ReturnBuffer(e);
                return;
            }

            ReceiveToken token = (ReceiveToken)e.UserToken;
            Socket playerSocket = null;
            try
            {
                // Try to get the client stream if it is still available
                playerSocket = token.player.TcpClientHandle.Client;
            }
            catch
            {
                // Failed to get the stream for the client so forcefully disconnect it
                //Console.WriteLine("Exception: Failed to get stream for client (Forcefully disconnecting)");
                Disconnect(token.player, true);
                ReturnBuffer(e);
                return;
            }

            // If the player is no longer connected, then make sure to disconnect it properly
            if (!token.player.TcpClientHandle.Connected)
            {
                Disconnect(token.player, false);
                ReturnBuffer(e);
                return;
            }

            // False means operation was synchronous (usually error)
            if (!playerSocket.ReceiveAsync(e))
                ReceiveAsync_Completed(this, e);
        }

        private void ReceiveAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                int bytesAlreadyProcessed = 0;
                ReceiveToken token = (ReceiveToken)e.UserToken;
                if (!token.player.Accepted && !token.player.Connected)
                {
                    byte[] header = HandleHttpHeader(e, ref bytesAlreadyProcessed);
                    if (header == null)
                    {
                        DoRead(e);
                        return;
                    }
                    byte[] response = Websockets.ValidateConnectionHeader(header);

                    // The response will be null if the header sent is invalid, if so then disconnect client as they are sending invalid headers
                    if (response == null)
                    {
                        OnPlayerRejected(token.player);
                        Disconnect(token.player, true);
                        ReturnBuffer(e);
                        return;
                    }

                    // If all is in order then send the validated response to the client
                    token.player.TcpClientHandle.GetStream().Write(response, 0, response.Length);

                    // The player has successfully connected
                    token.player.Connected = true;
                }
                while (bytesAlreadyProcessed < e.BytesTransferred)
                {
                    byte[] data = HandleData(e, true, ref bytesAlreadyProcessed);
                    if (data == null)
                    {
                        break;
                    }
                    FrameStream frame = Factory.DecodeMessage(data, true, MessageGroupIds.TCP_FIND_GROUP_ID, token.player);
                    if (!token.player.Accepted)
                    {
                        if (frame.GroupId == MessageGroupIds.NETWORK_ID_REQUEST)
                        {
                            token.player.InstanceGuid = ((Text)frame).ToString();

                            bool rejected;
                            OnPlayerGuidAssigned(token.player, out rejected);

                            // If the player was rejected during the handling of the playerGuidAssigned event, don't accept them.
                            if (rejected)
                                break;

                            token.maxAllowedBytes = int.MaxValue;

                            if (authenticator != null)
                            {
                                authenticator.IssueChallenge(this, token.player, IssueChallenge, AuthUser);
                            } else
                            {
                                AuthUser(token.player);
                            }
                        } else if (frame.GroupId == MessageGroupIds.AUTHENTICATION_RESPONSE)
                        {
                            // Authenticate user response
                            if (authenticator == null)
                                return;

                            authenticator.VerifyResponse(this, token.player, frame.StreamData, AuthUser, RejectUser);

                        } else
                        {
                            Disconnect(token.player, true);
                            ReturnBuffer(e);
                        }
                    }
                    else
                    {
                        token.player.Ping();
                        FireRead(frame, token.player);
                    }
                }
                DoRead(e);
            }
            else
            {
                Disconnect(((ReceiveToken)e.UserToken).player, true);
                ReturnBuffer(e);
            }
        }

        private void ReturnBuffer(SocketAsyncEventArgs e)
        {
            if (e.UserToken != null)
            {
                ReceiveToken token = (ReceiveToken)e.UserToken;
				bufferManager.ReturnBuffer(token.internalBuffer);
				token.internalBuffer = default(ArraySegment<byte>);
				e.SetBuffer(new byte[0], 0, 0);
			}
        }

        /// <summary>
        /// Callback for user auth. Sends an auth challenge to the user.
        /// </summary>
        private void IssueChallenge(NetworkingPlayer player, BMSByte buffer)
        {
            Send(player.TcpClientHandle, new Binary(Time.Timestep, false, buffer, Receivers.Target, MessageGroupIds.AUTHENTICATION_CHALLENGE, true));
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
            Send(player.TcpClientHandle, new Binary(Time.Timestep, false, buffer, Receivers.Target, MessageGroupIds.NETWORK_ID_REQUEST, true));
            SendBuffer(player);

            OnPlayerAccepted(player);
        }

        /// <summary>
        /// Callback for user auth. Sends an authentication failure message to the user and then disconnects them.
        /// </summary>
        private void RejectUser(NetworkingPlayer player)
        {
            OnPlayerRejected(player);
            Send(player.TcpClientHandle, Error.CreateErrorMessage(Time.Timestep, "Authentication Failed", false, MessageGroupIds.AUTHENTICATION_FAILURE, true));
            SendBuffer(player);
            Disconnect(player, true);
            CommitDisconnects();
        }

        /// <summary>
        /// Disconnects this server and all of it's clients
        /// </summary>
        /// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
        public override void Disconnect(bool forced)
        {
            // Since we are disconnecting we need to stop the read thread
            readThreadCancel = true;

            lock (Players)
            {
                // Stop listening for new connections
                listener.Stop();

                // Go through all of the players and disconnect them
                foreach (NetworkingPlayer player in Players)
                    Disconnect(player, true);

                // Send signals to the methods registered to the disconnec events
                if (!forced)
                    OnDisconnected();
                else
                    OnForcedDisconnect();
            }

            CommitDisconnects();
        }

        /// <summary>
        /// Disconnects a client from this listener
        /// </summary>
        /// <param name="client">The target client to be disconnected</param>
        public void Disconnect(NetworkingPlayer player, bool forced)
        {
            commonServerLogic.Disconnect(player, forced, DisconnectingPlayers, ForcedDisconnectingPlayers);
            if (!PendingCommitDisconnects)
            {
                PendingCommitDisconnects = true;
                Task.Queue(() => {
                    PendingCommitDisconnects = false;
                    CommitDisconnects();
                }, 30000);
            }
        }

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

            // Tell the player that he is getting disconnected
            if(player.TcpClientHandle != null && player.TcpClientHandle.Connected)
                Send(player.TcpClientHandle, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, true));

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
            OnPlayerDisconnected(player);
            if(player.TcpClientHandle != null)
                player.TcpClientHandle.Close();
            rawClients.Remove(player.TcpClientHandle);

            if (!forced)
            {
                // Let all of the event listeners know that the client has successfully disconnected
                if (rawClientDisconnected != null)
                    rawClientDisconnected(player.TcpClientHandle);
                DisconnectingPlayers.Remove(player);
            }
            else
            {
                // Let all of the event listeners know that this was a forced disconnect
                if (forced && rawClientForceClosed != null)
                    rawClientForceClosed(player.TcpClientHandle);
                ForcedDisconnectingPlayers.Remove(player);
            }
        }

#if WINDOWS_UWP
		private void ClientRejected(StreamSocket client, bool forced)
#else
        private void ClientRejected(TcpClient client, bool forced)
#endif
        {
            // Clean up the client objects
            client.Close();
        }

        private void SendBuffer(NetworkingPlayer player)
        {
            foreach (FrameStream frame in bufferedMessages)
                Send(player.TcpClientHandle, frame);
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
            SendToPlayer(GeneratePong(time), playerRequesting);
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

        public override void FireRead(FrameStream frame, NetworkingPlayer currentPlayer)
        {
            // The client has told the server that it is disconnecting
            if (frame is ConnectionClose)
            {
                // Confirm the connection close
                Send(currentPlayer.TcpClientHandle, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, true));

                Disconnect(currentPlayer, false);
                CommitDisconnects();
                return;
            }

            // A message has been successfully read from the network so relay that
            // to all methods registered to the event
            OnMessageReceived(currentPlayer, frame);
        }
    }
}

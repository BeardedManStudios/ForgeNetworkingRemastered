using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Threading;

namespace BeardedManStudios.Forge.Networking
{
    public abstract class  ETCPClientBase : EBaseTCP, IClient
    {
#if WINDOWS_UWP
		private StreamSocket client
#else
        private TcpClient client;
#endif
        /// <summary>
        /// The hash that is / was validated by the server
        /// </summary>
        private string headerHash;

        /// <summary>
        /// Used to determine if the client has requested to be accepted by the server
        /// </summary>
        private bool headerExchanged = false;

        /// <summary>
        /// Whether we are disconnected or not
        /// </summary>
        private bool disconnectedSelf = false;

        public event BaseNetworkEvent ConnectAttemptFailed;
        byte[] buffer = new byte[8192];

        /// <summary>
		/// The identity of the server as a networking player
		/// </summary>
        protected NetworkingPlayer server = null;
        public NetworkingPlayer Server { get { return server; } }

        public virtual void Connect(string host, ushort port = DEFAULT_PORT)
        {
            try
            {
                client = new TcpClient(host, port); // constructor runs connect
            }
            catch
            {
                if(ConnectAttemptFailed != null)
                {
                    ConnectAttemptFailed(this);
                }
                return;
            }
            // If we got this far then the bind was successful
            OnBindSuccessful();
            Initialize(host, port);
        }
        protected virtual void Initialize(string host, ushort port)
        {
            // Get a random hash key that needs to be used for validating that the server was connected to
            headerHash = Websockets.HeaderHashKey();

            // This is a typical Websockets accept header to be validated
            byte[] connectionHeader = Websockets.ConnectionHeader(headerHash, port);

            // Register the server as a NetworkingPlayer
            server = new NetworkingPlayer(0, host, true, client, this);
            // Send the upgrade request to the server
            RawWrite(connectionHeader);

            //Let myself know I connected successfully
            OnPlayerConnected(server);
            // Set myself as a connected client
            server.Connected = true;

            ReceiveToken token = new ReceiveToken
            {
                internalBuffer = new ArraySegment<byte>(buffer, 0, buffer.Length),
                player = server,
                bytesReceived = 0,
                dataHolder = null,
                maxAllowedBytes = 8192
            };

            // Read from the server async
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveAsync_Completed);
            e.UserToken = token;
            e.SetBuffer(token.internalBuffer.Array, token.internalBuffer.Offset, token.internalBuffer.Count);

            if (!client.Client.ReceiveAsync(e))
                Task.Queue(() => ReceiveAsync_Completed(this, e));
        }

        private void DoRead(SocketAsyncEventArgs e)
        {
            if (!client.Client.ReceiveAsync(e))
                ReceiveAsync_Completed(this, e);
        }

        // Should validate the handshake response from the server
        private void ReceiveAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                int bytesAlreadyProcessed = 0; // Count of the total freshly transferred bytes processed so far
                ReceiveToken token = (ReceiveToken)e.UserToken;
                if (!headerExchanged)
                {
                    byte[] header = HandleHttpHeader(e, ref bytesAlreadyProcessed);
                    token = (ReceiveToken)e.UserToken;
                    if (header == null)
                    {
                        DoRead(e);
                        return;
                    } else if (Websockets.ValidateResponseHeader(headerHash, header))
                    {
                        headerExchanged = true;
                        token.maxAllowedBytes = int.MaxValue;
                        e.UserToken = token;

                        // Ping the server to finalize the player's connection
                        Send(Text.CreateFromString(Time.Timestep, InstanceGuid.ToString(), true, Receivers.Server, MessageGroupIds.NETWORK_ID_REQUEST, true));
                    } else
                    {
                        // Improper header, so a disconnect is required
                        Disconnect(true);
                        return;
                    }
                }

                while (bytesAlreadyProcessed < e.BytesTransferred)
                {
                    byte[] data = HandleData(e, true, ref bytesAlreadyProcessed);
                    if (data == null)
                    {
                        break;
                    }
                    FrameStream frame = Factory.DecodeMessage(data, false, MessageGroupIds.TCP_FIND_GROUP_ID, Server);

                    FireRead(frame, Server);
                    
                }
                DoRead(e);
            } else
            {
                Disconnect(true);
            }
        }
        private void RawWrite(byte[] data)
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
        public virtual void Send(FrameStream frame)
        {
            // Make sure that we don't have any race conditions with writing to the same client
            lock (client)
            {
                // Get the raw bytes from the frame and send them
                byte[] data = frame.GetData();
                RawWrite(data);
            }
        }

        /// <summary>
        /// Sends binary message to the specific receiver(s)
        /// </summary>
        /// <param name="receivers">The clients / server to receive the message</param>
        /// <param name="messageGroupId">The Binary.GroupId of the massage, use MessageGroupIds.START_OF_GENERIC_IDS + desired_id</param>
        /// <param name="objectsToSend">Array of vars to be sent, read them with Binary.StreamData.GetBasicType<typeOfObject>()</param>
        public virtual void Send(Receivers receivers, int messageGroupId, params object[] objectsToSend)
        {
            BMSByte data = ObjectMapper.BMSByte(objectsToSend);
            Binary sendFrame = new Binary(Time.Timestep, true, data, receivers, messageGroupId, true);
            Send(sendFrame);
        }

        /// <summary>
        /// Disconnect this client from the server
        /// </summary>
        /// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
        public override void Disconnect(bool forced)
        {
            if (client != null)
            {
                lock (client)
                {
                    disconnectedSelf = true;

                    // Close our TcpClient so that it can no longer be used
                    if (forced)
                        client.Close();
                    else
                        Send(new ConnectionClose(Time.Timestep, true, Receivers.Server, MessageGroupIds.DISCONNECT, true));

                    // Send signals to the methods registered to the disconnec events
                    if (!forced)
                        OnDisconnected();
                    else
                        OnForcedDisconnect();

                    for (int i = 0; i < Players.Count; ++i)
                        OnPlayerDisconnected(Players[i]);
                }
            }
        }

        public override void FireRead(FrameStream frame, NetworkingPlayer currentPlayer)
        {
            // A message has been successfully read from the network so relay that
            // to all methods registered to the event
            OnMessageReceived(currentPlayer, frame);
        }

        /// <summary>
        /// Request the ping from the server (pingReceived will be triggered if it receives it)
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
    }
}

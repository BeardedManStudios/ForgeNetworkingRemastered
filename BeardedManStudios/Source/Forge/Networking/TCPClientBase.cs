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
using System;

#if WINDOWS_UWP
using Windows.Networking.Sockets;
using Windows.Networking;
using System.IO;
#else
using System.Net.Sockets;
#endif

namespace BeardedManStudios.Forge.Networking
{
	public abstract class TCPClientBase : BaseTCP, IClient
	{
		/// <summary>
		/// The reference to the raw client that is connected to the server
		/// </summary>
#if WINDOWS_UWP
		public StreamSocket client { get; private set; }
#else
		public TcpClient client { get; private set; }
#endif

		/// <summary>
		/// The hash that is / was validated by the server
		/// </summary>
		private string headerHash = string.Empty;

		/// <summary>
		/// Used to determine if the client has requested to be accepted by the server
		/// </summary>
		private bool headerExchanged = false;

		/// <summary>
		/// Whether we are disconnected or not
		/// </summary>
		private bool disconnectedSelf = false;

		/// <summary>
		/// The identity of the server as a player
		/// </summary>
		protected NetworkingPlayer server = null;
		public NetworkingPlayer Server { get { return server; } }

		public event BaseNetworkEvent connectAttemptFailed;

		/// <summary>
		/// Used to determine what happened during the read method and what actions
		/// that should be taken if it was called from an infinite loop thread
		/// </summary>
		protected enum ReadState
		{
			Void,
			Disconnect,
			Continue
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
		/// This will begin the connection for TCP, this is a thread blocking operation
		/// until the connection is either established or has failed
		/// </summary>
		/// <param name="hostAddress">Ip Address to connect to</param>
		/// <param name="port">[15937] Port connect to on the server</param>
#if WINDOWS_UWP
		public async virtual void Connect(string host, ushort port = DEFAULT_PORT)
#else
		public virtual void Connect(string host, ushort port = DEFAULT_PORT)
#endif
		{
			try
			{
				disconnectedSelf = false;
				// Create the raw TcpClient that is to communicate with the server's tcp listener
#if WINDOWS_UWP
				client = new StreamSocket();
				await client.ConnectAsync(new HostName(host), port.ToString());
#else
				try
				{
					client = new TcpClient(host, port);
				}
				catch
				{
					if (connectAttemptFailed != null)
						connectAttemptFailed();

					return;
				}
#endif

				// This was a successful bind so fire the events for it
				OnBindSuccessful();

				Initialize(host, port);
			}
			catch (Exception e)
			{
				// This was a failed bind so fire the events for it
				OnBindFailure();

				throw new BaseNetworkException("Host is invalid or not found", e);
			}
		}

		/// <summary>
		/// Setup everything required for this client to be accepted by the server and
		/// construct / send the connection header
		/// </summary>
		/// <param name="port">The port that was connected to on the remote host</param>
		protected virtual void Initialize(string host, ushort port)
		{
			// By default pending creates should be true and flushed when ready
			PendCreates = true;

			// Get a client stream for reading and writing. 
			// Stream stream = client.GetStream();

			// Get a random hash key that needs to be used for validating that the server was connected to
			headerHash = Websockets.HeaderHashKey();

			// This is a typical Websockets accept header to be validated
			byte[] connectHeader = Websockets.ConnectionHeader(headerHash, port);

			// Send the accept headers to the server to validate
			RawWrite(connectHeader);

			// Setup the identity of the server as a player
			server = new NetworkingPlayer(0, host, true, client, this);

			//Let myself know I connected successfully
			OnPlayerConnected(server);
			// Set myself as a connected client
			server.Connected = true;
		}

		protected void InitializeMasterClient(string host, ushort port)
		{
			// Get a client stream for reading and writing. 
			// Stream stream = client.GetStream();

			// Get a random hash key that needs to be used for validating that the server was connected to
			headerHash = Websockets.HeaderHashKey();

			// This is a typical Websockets accept header to be validated
			byte[] connectHeader = Websockets.ConnectionHeader(headerHash, port);

			// Send the accept headers to the server to validate
			RawWrite(connectHeader);

			// Setup the identity of the server as a player
			server = new NetworkingPlayer(0, host, true, client, this);

			//Let myself know I connected successfully
			OnPlayerConnected(server);
			// Set myself as a connected client
			server.Connected = true;
		}

		/// <summary>
		/// Infinite loop listening for new data from all connected clients on a separate thread.
		/// This loop breaks when readThreadCancel is set to true
		/// </summary>
		protected ReadState Read()
		{
			if (disconnectedSelf)
				return ReadState.Disconnect;

			NetworkStream stream = client.GetStream();

			if (stream == null) //Some reason the stream is null!
				return ReadState.Continue;

			// If the stream no longer can read then disconnect
			if (!stream.CanRead)
			{
				Disconnect(true);
				return ReadState.Disconnect;
			}

			// If there isn't any data available, then free up the CPU by sleeping the thread
			if (!stream.DataAvailable)
				return ReadState.Continue;

			int available = client.Available;

			if (available == 0)
				return ReadState.Continue;

			// Determine if this client has been accepted by the server yet
			if (!headerExchanged)
			{
				// Read all available bytes in the stream as this client hasn't connected yet
				byte[] bytes = new byte[available];
				stream.Read(bytes, 0, bytes.Length);

				// The first packet response from the server is going to be a string
				if (Websockets.ValidateResponseHeader(headerHash, bytes))
				{
					headerExchanged = true;

					// Ping the server to finalize the player's connection
					Send(Text.CreateFromString(Time.Timestep, InstanceGuid.ToString(), true, Receivers.Server, MessageGroupIds.NETWORK_ID_REQUEST, true));
				}
				else
				{
					// Improper header, so a disconnect is required
					Disconnect(true);
					return ReadState.Disconnect;
				}
			}
			else
			{
				byte[] messageBytes = GetNextBytes(stream, available, false);

				// Get the frame that was sent by the server, the server
				// does not send masked data, only the client so send false for mask
				FrameStream frame = Factory.DecodeMessage(messageBytes, false, MessageGroupIds.TCP_FIND_GROUP_ID, Server);

				if (frame is ConnectionClose)
				{
					// Close our CachedUDPClient so that it can no longer be used
					client.Close();
					return ReadState.Disconnect;
				}

				// A message has been successfully read from the network so relay that
				// to all methods registered to the event
				OnMessageReceived(Server, frame);
			}

			return ReadState.Void;
		}

		/// <summary>
		/// Disconnect this client from the server
		/// </summary>
		/// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
		public override void Disconnect(bool forced)
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



		/// <summary>
		/// Request the ping from the server (pingReceived will be triggered if it receives it)
		/// </summary>
		public override void Ping()
		{
			Send(GeneratePing());
		}
	}
}
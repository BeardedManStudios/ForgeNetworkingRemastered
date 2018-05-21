﻿/*-----------------------------+-------------------------------\
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
using System.Threading;
using System.Linq;

#if WINDOWS_UWP
using Windows.Networking.Sockets;
using System.IO;
#else
using System.Net.Sockets;
#endif

namespace BeardedManStudios.Forge.Networking
{
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


		public TCPServer(int maxConnections) : base(maxConnections)
		{
			AcceptingConnections = true;
			BannedAddresses = new List<string>();
			commonServerLogic = new CommonServerLogic(this);
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
						if (frame.Receivers == Receivers.Others || frame.Receivers == Receivers.OthersBuffered || frame.Receivers == Receivers.OthersProximity)
							return;
					}

					// Check to see if the request is based on proximity
					if (frame.Receivers == Receivers.AllProximity || frame.Receivers == Receivers.OthersProximity)
					{
						// If the target player is not in the same proximity zone as the sender
						// then it should not be sent to that player
						if (player.ProximityLocation.Distance(frame.Sender.ProximityLocation) > ProximityDistance)
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
					if (!commonServerLogic.PlayerIsReceiver(player, frame, ProximityDistance, skipPlayer))
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
				throw new ObjectDisposedException("TCPServer", "This object has been disposed and can not be used to connect, please use a new TCPServer");

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

			// Clients will be looped through on other threads so be sure to lock it before adding
			lock (Players)
			{
				rawClients.Add(client);

				// Create the identity wrapper for this player
				NetworkingPlayer player = new NetworkingPlayer(ServerPlayerCounter++, client.Client.RemoteEndPoint.ToString(), false, client, this);

				// Generically add the player and fire off the events attached to player joining
				OnPlayerConnected(player);
			}

			// Let all of the event listeners know that the client has successfully connected
			if (rawClientConnected != null)
				rawClientConnected(client);
		}
		/// <summary>
		/// Infinite loop listening for new data from all connected clients on a separate thread.
		/// This loop breaks when readThreadCancel is set to true
		/// </summary>
		private void ReadClients()
		{
			// Intentional infinite loop
			while (IsBound && !NetWorker.EndingSession)
			{
				try
				{
					// If the read has been flagged to be canceled then break from this loop
					if (readThreadCancel)
						return;

					// This will loop through all of the players, so make sure to set the lock to
					// prevent any changes from other threads
					lock (Players)
					{
						for (int i = 0; i < Players.Count; i++)
						{
							// If the read has been flagged to be canceled then break from this loop
							if (readThreadCancel)
								return;

							NetworkStream playerStream = null;

							if (Players[i].IsHost)
								continue;

							try
							{
								lock (Players[i].MutexLock)
								{
									// Try to get the client stream if it is still available
									playerStream = Players[i].TcpClientHandle.GetStream();
								}
							}
							catch
							{
								// Failed to get the stream for the client so forcefully disconnect it
								//Console.WriteLine("Exception: Failed to get stream for client (Forcefully disconnecting)");
								Disconnect(Players[i], true);
								continue;
							}

							// If the player is no longer connected, then make sure to disconnect it properly
							if (!Players[i].TcpClientHandle.Connected)
							{
								Disconnect(Players[i], false);
								continue;
							}

							// Only continue to read for this client if there is any data available for it
							if (!playerStream.DataAvailable)
								continue;

							int available = Players[i].TcpClientHandle.Available;

							// Determine if the player is fully connected
							if (!Players[i].Accepted)
							{
								// Determine if the player has been accepted by the server
								if (!Players[i].Connected)
								{
									lock (Players[i].MutexLock)
									{
										// Read everything from the stream as the client hasn't been accepted yet
										byte[] bytes = new byte[available];
										playerStream.Read(bytes, 0, bytes.Length);

										// Validate that the connection headers are properly formatted
										byte[] response = Websockets.ValidateConnectionHeader(bytes);

										// The response will be null if the header sent is invalid, if so then disconnect client as they are sending invalid headers
										if (response == null)
										{
											OnPlayerRejected(Players[i]);
											Disconnect(Players[i], false);
											continue;
										}

										// If all is in order then send the validated response to the client
										playerStream.Write(response, 0, response.Length);

										// The player has successfully connected
										Players[i].Connected = true;
									}
								}
								else
								{
									lock (Players[i].MutexLock)
									{
										// Consume the message even though it is not being used so that it is removed from the buffer
										Text frame = (Text)Factory.DecodeMessage(GetNextBytes(playerStream, available, true), true, MessageGroupIds.TCP_FIND_GROUP_ID, Players[i]);
										Players[i].InstanceGuid = frame.ToString();

										OnPlayerGuidAssigned(Players[i]);

										lock (writeBuffer)
										{
											writeBuffer.Clear();
											writeBuffer.Append(BitConverter.GetBytes(Players[i].NetworkId));
											Send(Players[i].TcpClientHandle, new Binary(Time.Timestep, false, writeBuffer, Receivers.Target, MessageGroupIds.NETWORK_ID_REQUEST, true));

											SendBuffer(Players[i]);

											// All systems go, the player has been accepted
											OnPlayerAccepted(Players[i]);
										}
									}
								}
							}
							else
							{
								try
								{
									lock (Players[i].MutexLock)
									{
										Players[i].Ping();

										// Get the frame that was sent by the client, the client
										// does send masked data
										//TODO: THIS IS CAUSING ISSUES!!! WHY!?!?!!?
										FrameStream frame = Factory.DecodeMessage(GetNextBytes(playerStream, available, true), true, MessageGroupIds.TCP_FIND_GROUP_ID, Players[i]);

										// The client has told the server that it is disconnecting
										if (frame is ConnectionClose)
										{
											// Confirm the connection close
											Send(Players[i].TcpClientHandle, new ConnectionClose(Time.Timestep, false, Receivers.Target, MessageGroupIds.DISCONNECT, true));

											Disconnect(Players[i], false);
											continue;
										}

										FireRead(frame, Players[i]);
									}
								}
								catch
								{
									// The player is sending invalid data so disconnect them
									Disconnect(Players[i], true);
								}
							}
						}
					}

					// Go through all of the pending disconnections and clean them
					// up and finalize the disconnection
					CleanupDisconnections();

					// Sleep so that we free up the CPU a bit from this thread
					Thread.Sleep(10);
				}
				catch (Exception ex)
				{
					Logging.BMSLog.LogException(ex);
				}
			}
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
					Disconnect(player, forced);

				// Send signals to the methods registered to the disconnec events
				if (!forced)
					OnDisconnected();
				else
					OnForcedDisconnect();
			}
		}

		/// <summary>
		/// Disconnects a client from this listener
		/// </summary>
		/// <param name="client">The target client to be disconnected</param>
		public void Disconnect(NetworkingPlayer player, bool forced)
		{
			commonServerLogic.Disconnect(player, forced, DisconnectingPlayers, ForcedDisconnectingPlayers);
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
			// A message has been successfully read from the network so relay that
			// to all methods registered to the event
			OnMessageReceived(currentPlayer, frame);
		}
	}
}
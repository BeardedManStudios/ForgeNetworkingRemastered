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
using System.Net.Sockets;
using System.Threading;

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

		private List<UDPPacketComposer> pendingComposers = new List<UDPPacketComposer>();

		public NatHolePunch nat = new NatHolePunch();

		public event BaseNetworkEvent connectAttemptFailed;

		public override void Send(FrameStream frame, bool reliable = false)
		{
			UDPPacketComposer composer = new UDPPacketComposer();

			// If this message is reliable then make sure to keep a reference to the composer
			// so that there are not any run-away threads
			if (reliable)
			{
				// Use the completed event to clean up the object from memory
				composer.completed += ComposerCompleted;
				pendingComposers.Add(composer);
			}

			//TODO: New constructor for setting up callbacks before regular constructor (as seen above)
			composer.Init(this, Server, frame, reliable);
		}

		/// <summary>
		/// Used to clean up the target composer from memory
		/// </summary>
		/// <param name="composer">The composer that has completed</param>
		private void ComposerCompleted(UDPPacketComposer composer)
		{
			pendingComposers.Remove(composer);
		}

		public void Connect(string host, ushort port = DEFAULT_PORT, string natHost = "", ushort natPort = NatHolePunch.DEFAULT_NAT_SERVER_PORT, bool isSpecial = false)
		{
			// By default pending creates should be true and flushed when ready
			if (!isSpecial)
				PendCreates = true;

			try
			{
				// TODO:  Remove + 1, it is for linux tests
				ushort clientPort = port;//(ushort)(port + 1);
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

				Client.EnableBroadcast = true;

				// If the server is behind a NAT, request for the port punch by the nat server
				if (!string.IsNullOrEmpty(natHost))
					nat.Connect(host, port, clientPort, natHost, natPort);

				// Do any generic initialization in result of the successful bind
				OnBindSuccessful();

				// Get a random hash key that needs to be used for validating that the server was connected to
				headerHash = Websockets.HeaderHashKey();

				// This is a typical Websockets accept header to be validated
				byte[] connectHeader = Websockets.ConnectionHeader(headerHash, port);

				// Setup the identity of the server as a player
				server = new NetworkingPlayer(0, host, true, ResolveHost(host, port), this);

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
						Client.Send(connectHeader, connectHeader.Length, Server.IPEndPointHandle);
						Thread.Sleep(3000);
					} while (!headerExchanged && IsBound && ++connectCounter < CONNECT_TRIES);

					if (connectCounter >= CONNECT_TRIES)
					{
						if (connectAttemptFailed != null)
							connectAttemptFailed();
					}
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
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);
			string incomingEndpoint = string.Empty;

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
						// Read a packet from the network
						packet = Client.Receive(ref groupEP, ref incomingEndpoint);

						if (PacketLossSimulation > 0.0f && new Random().NextDouble() <= PacketLossSimulation)
						{
							// Skip this message
							continue;
						}

						BandwidthIn += (ulong)packet.Size;
					}
					catch (SocketException /*ex*/)
					{
						// This is a common exception when we exit the blocking call
						//Logging.BMSLog.LogException(ex);
						Disconnect(true);
					}

					// This message was not from the server
					if (groupEP.Address != Server.IPEndPointHandle.Address &&
						groupEP.Port != Server.IPEndPointHandle.Port)
					{
						if (packet.Size == 1 && (packet[0] == SERVER_BROADCAST_CODE || packet[1] == CLIENT_BROADCAST_CODE))
						{

						}
						else if (packet.Size.Between(2, 4) && packet[0] == BROADCAST_LISTING_REQUEST_1 && packet[1] == BROADCAST_LISTING_REQUEST_2 && packet[2] == BROADCAST_LISTING_REQUEST_3)
						{
							// This may be a local listing request so respond with the client flag byte
							Client.Send(new byte[] { CLIENT_BROADCAST_CODE }, 1, groupEP);
						}

						continue;
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

						// Add the packet to the manager so that it can be tracked and executed on complete
						packetManager.AddPacket(formattedPacket, PacketSequenceComplete);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.BMSLog.LogException(ex);
				Disconnect(true);
			}
		}

		private void PacketSequenceComplete(BMSByte data, int groupId, byte receivers)
		{
			// Pull the frame from the sent message
			FrameStream frame = Factory.DecodeMessage(data.CompressBytes(), false, groupId, Server, receivers);

			if (frame is ConnectionClose)
			{
				CloseConnection();
				return;
			}

			// Send an event off that a packet has been read
			OnMessageReceived(Server, frame);
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
	}
}
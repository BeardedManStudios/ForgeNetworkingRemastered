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
#if STEAMWORKS
using Steamworks;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Net;

#if WINDOWS_UWP
using Windows.Networking.Sockets;
#else
using System.Net.Sockets;
#endif

namespace BeardedManStudios.Forge.Networking
{
	public class NetworkingPlayer
	{
		private const uint PLAYER_TIMEOUT_DISCONNECT = 30000;
		private const int DEFAULT_PING_INTERVAL = 5000;

		/// <summary>
		/// An event that is called whenever this player has disconnected
		/// </summary>
		public event NetWorker.BaseNetworkEvent disconnected;

		/// <summary>
		/// The socket to the Networking player
		/// </summary>
		public object SocketEndpoint { get; private set; }

		/// <summary>
		/// A reference to the raw tcp listener for this player (only used on server)
		/// </summary>
#if WINDOWS_UWP
		public StreamSocketListener TcpListenerHandle { get; private set; }
#else
		public TcpListener TcpListenerHandle { get; private set; }
#endif

		/// <summary>
		/// A reference to the raw tcp client for this player
		/// </summary>
#if WINDOWS_UWP
		public StreamSocket TcpClientHandle { get; private set; }
#else
		public TcpClient TcpClientHandle { get; private set; }
#endif

		/// <summary>
		/// A reference to the IPEndPoint for this player
		/// </summary>
		public IPEndPoint IPEndPointHandle { get; private set; }

		/// <summary>
		/// The NetworkID the NetworkingPlayer is
		/// </summary>
		public uint NetworkId { get; private set; }

		/// <summary>
		/// IP address of the NetworkingPlayer
		/// </summary>
		public string Ip { get; private set; }

		/// <summary>
		/// Port number of this NetworkingPlayer
		/// </summary>
		public ushort Port { get; private set; }

		/// <summary>
		/// Name of the NetworkingPlayer
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Determines if the player has been accepted for the connection by the server
		/// </summary>
		public bool Accepted { get; set; }

		/// <summary>
		/// Determines if the player has been sent an accept request but the server
		/// is still waiting on a confirmation of the acceptance
		/// </summary>
		public bool PendingAccepted { get; set; }

        /// <summary>
        /// Determines if the player has been authenticated by the server
        /// </summary>
        public bool Authenticated { get; set; }

        /// <summary>
        /// Determines if the player is currently connected
        /// </summary>
        public bool Connected { get; set; }

		/// <summary>
		/// Is set once a disconnection happens
		/// </summary>
		public bool Disconnected { get; private set; }

		/// <summary>
		/// This is the message group that this particular player is a part of
		/// </summary>
		public ushort MessageGroup { get; private set; }

		/// <summary>
		/// Last ping sent to the NetworkingPlayer
		/// </summary>
		public ulong LastPing { get; private set; }

		/// <summary>
		/// Whether this player is the one hosting
		/// </summary>
		public bool IsHost { get; private set; }

		/// <summary>
		/// Used to determine if this player is in the process of disconnecting
		/// </summary>
		public bool IsDisconnecting { get; set; }

		/// <summary>
		/// Whether we are locked
		/// </summary>
		public object MutexLock = new object();

		/// <summary>
		/// Keep a list of all of the composers that are reliable so that they are sent in order
		/// </summary>
		private List<BasePacketComposer> reliableComposers = new List<BasePacketComposer>();

		/// <summary>
		/// Should be used for matching this networking player with another networking player reference
		/// on a different networker.
		/// </summary>
		public string InstanceGuid { get; set; }

		/// <summary>
		/// The amount of time in seconds to disconnect this player if no messages are sent
		/// </summary>
		public uint TimeoutMilliseconds { get; set; }

		private bool composerReady = false;

		private int currentPingWait = 0;
		public int PingInterval { get; set; }

		/// <summary>
		/// The amount of time it took for a ping to happen
		/// </summary>
		public int RoundTripLatency { get; set; }

		public NetWorker Networker { get; private set; }

		/// <summary>
		/// This is used for proximity based updates, this should update with
		/// the player location to properly be used with the NetWorker::ProximityDistance
		/// </summary>
		public Vector ProximityLocation { get; set; }

        public GridLocation gridPosition { get; set; }

        /// <summary>
        /// Used to match players proximity status against each player, to know how many times
        ///  updating him has been skipped - used with the NetWorker::ProximityDistance
        /// </summary>
        public Dictionary<string, int> PlayersProximityUpdateCounters = new Dictionary<string, int>();


        private ulong currentReliableId = 0;
		public Dictionary<ulong, FrameStream> reliablePending = new Dictionary<ulong, FrameStream>();

		public ulong UniqueReliableMessageIdCounter { get; private set; }

#if STEAMWORKS
        /// <summary>
        /// This is used for steam networking API calls;
        /// this is the steam ID of this networked player.
        /// </summary>
        public CSteamID SteamID { get; protected set; }

        public void AssignOwnSteamId()
        {
            SteamID = SteamUser.GetSteamID();
        }
#endif

        private Queue<ulong> reliableComposersToRemove = new Queue<ulong>();

        /// <summary>
        /// Constructor for the NetworkingPlayer
        /// </summary>
        /// <param name="networkId">NetworkId set for the NetworkingPlayer</param>
        /// <param name="ip">IP address of the NetworkingPlayer</param>
        /// <param name="socketEndpoint">The socket to the Networking player</param>
        /// <param name="name">Name of the NetworkingPlayer</param>
        public NetworkingPlayer(uint networkId, string ip, bool isHost, object socketEndpoint, NetWorker networker)
		{
			Networker = networker;
			NetworkId = networkId;
			Ip = ip.Split('+')[0];
			IsHost = isHost;
			SocketEndpoint = socketEndpoint;
			LastPing = networker.Time.Timestep;
			TimeoutMilliseconds = PLAYER_TIMEOUT_DISCONNECT;
			PingInterval = DEFAULT_PING_INTERVAL;

			if (SocketEndpoint != null)
			{
#if WINDOWS_UWP
				// Check to see if the supplied socket endpoint is TCP, if so
				// assign it to the TcpClientHandle for ease of access
				if (socketEndpoint is StreamSocket)
				{
					TcpClientHandle = (StreamSocket)socketEndpoint;
					IPEndPointHandle = (IPEndPoint)TcpClientHandle.Client.RemoteEndPoint;
				}
				else if (socketEndpoint is StreamSocketListener)
				{
					TcpListenerHandle = (StreamSocketListener)socketEndpoint;
					IPEndPointHandle = (IPEndPoint)TcpListenerHandle.LocalEndpoint;
				}
				else if (SocketEndpoint is IPEndPoint)
					IPEndPointHandle = (IPEndPoint)SocketEndpoint;
#else
				// Check to see if the supplied socket endpoint is TCP, if so
				// assign it to the TcpClientHandle for ease of access
				if (socketEndpoint is TcpClient)
				{
					TcpClientHandle = (TcpClient)socketEndpoint;
					IPEndPointHandle = (IPEndPoint)TcpClientHandle.Client.RemoteEndPoint;
				}
				else if (socketEndpoint is TcpListener)
				{
					TcpListenerHandle = (TcpListener)socketEndpoint;
					IPEndPointHandle = (IPEndPoint)TcpListenerHandle.LocalEndpoint;
				}
				else if (SocketEndpoint is IPEndPoint)
					IPEndPointHandle = (IPEndPoint)SocketEndpoint;
#endif

				Port = (ushort)IPEndPointHandle.Port;
			}
		}

#if STEAMWORKS
        public NetworkingPlayer(uint networkId, CSteamID steamId, bool isHost, NetWorker networker)
        {
            SteamID = steamId;
            Networker = networker;
            NetworkId = networkId;
            IsHost = isHost;
            LastPing = networker.Time.Timestep;
            TimeoutMilliseconds = PLAYER_TIMEOUT_DISCONNECT;
            PingInterval = DEFAULT_PING_INTERVAL;
        }
#endif

        public void AssignPort(ushort port)
		{
			// Only allow to be assigned once
			if (Port != 0)
				return;

			Port = port;
		}

		/// <summary>
		/// Ping the NetworkingPlayer
		/// </summary>
		public void Ping()
		{
			LastPing = Networker.Time.Timestep;
		}

		/// <summary>
		/// Called by the server to check and see if this player has timed out
		/// </summary>
		/// <returns>True if the player has timed out</returns>
		public bool TimedOut()
		{
			return LastPing + TimeoutMilliseconds <= Networker.Time.Timestep;
		}

		/// <summary>
		/// Assigns the message group for this player
		/// </summary>
		/// <param name="messageGroup">The numerical identifier of the message group</param>
		public void SetMessageGroup(ushort messageGroup)
		{
			MessageGroup = messageGroup;
		}

		public void OnDisconnect()
		{
			Disconnected = true;
			Connected = false;

			StopComposers();

			if (disconnected != null)
				disconnected(Networker);
		}

        public void QueueComposer(BasePacketComposer composer)
        {
			if (Disconnected)
				return;

			lock (reliableComposers)
			{
				reliableComposers.Add(composer);
			}

			// Start the reliable send thread on this composer
			NextComposerInQueue();
		}

		/// <summary>
		/// Star the next composer available composer
		/// </summary>
		private void NextComposerInQueue()
		{
			// If there are not currently any queued composers then we can stop here
			if (reliableComposers.Count == 0)
				return;

			if (!composerReady && Networker.IsBound && !NetWorker.EndingSession)
			{
				composerReady = true;

				// Run this on a separate thread so that it doesn't interfere with the reading thread
				Task.Queue(() =>
				{
					int waitTime = 10, composerCount = 0;
					while (Networker.IsBound && !Disconnected)
					{
						lock (reliableComposers)
						{
							composerCount = reliableComposers.Count;
						}

						if (composerCount == 0)
						{
							Task.Sleep(waitTime);
							currentPingWait += waitTime;

							if (!(Networker is IServer) && currentPingWait >= PingInterval)
							{
								currentPingWait = 0;
								Networker.Ping();
							}

							continue;
						}

						do
						{
							// If there are too many packets to send, be sure to only send
							// a few to not clog the network.
							int counter = UDPPacketComposer.PACKET_SIZE;

							// Send all the packets that are pending
							lock (reliableComposers)
							{
								for (int i = 0; i < reliableComposers.Count; i++)
									reliableComposers[i].ResendPackets(Networker.Time.Timestep, ref counter);
							}

							Task.Sleep(10);

							// Check if we have some composers queued to remove
							lock (reliableComposersToRemove)
							{
								while (reliableComposersToRemove.Count > 0)
								{
									// Remove 
									lock (reliableComposers)
									{
										ulong id = reliableComposersToRemove.Dequeue();
										reliableComposers.Remove(reliableComposers.First(r => r.Frame.UniqueId == id));
									}
								}
							}

							lock (reliableComposers)
							{
								composerCount = reliableComposers.Count;
							}
						} while (composerCount > 0 && Networker.IsBound && !NetWorker.EndingSession);
						currentPingWait = 0;
					}
				});
			}
		}

		/// <summary>
		/// Cleans up the current composer and prepares to start up the next in the queue
		/// </summary>
		public void CleanupComposer(ulong uniqueId)
		{
			lock (reliableComposers)
			{
				reliableComposers.Remove(reliableComposers.First(r => r.Frame.UniqueId == uniqueId));
			}
		}

		/// <summary>
		/// Add composer to the queue, so it will be removed by sending thread
		/// </summary>
		public void EnqueueComposerToRemove(ulong uniqueId)
		{
			lock (reliableComposersToRemove)
			{
				reliableComposersToRemove.Enqueue(uniqueId);
			}
		}

		/// <summary>
		/// Go through and stop all of the reliable composers for this player to prevent
		/// them from being sent
		/// </summary>
		public void StopComposers()
		{
			lock (reliableComposers)
			{
				reliableComposers.Clear();
			}
		}

		public void WaitReliable(FrameStream frame)
		{
			if (!frame.IsReliable)
				return;

			if (frame.UniqueReliableId == currentReliableId)
			{
				Networker.FireRead(frame, this);
				currentReliableId++;

				FrameStream next = null;
				while (true)
				{
					if (!reliablePending.TryGetValue(currentReliableId, out next))
						break;

					reliablePending.Remove(currentReliableId++);
					Networker.FireRead(next, this);
				}
			}
			else if (frame.UniqueReliableId > currentReliableId)
				reliablePending.Add(frame.UniqueReliableId, frame);
		}

		public ulong GetNextReliableId()
		{
			return UniqueReliableMessageIdCounter++;
		}
	}
}

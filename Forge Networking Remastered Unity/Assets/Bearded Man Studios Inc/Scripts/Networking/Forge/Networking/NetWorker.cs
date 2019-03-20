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

using BeardedManStudios.Forge.Networking.DataStore;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Source.Forge.Networking;
using BeardedManStudios.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Ping = BeardedManStudios.Forge.Networking.Frame.Ping;

namespace BeardedManStudios.Forge.Networking
{
	public abstract class NetWorker
	{
		public const byte SERVER_BROADCAST_CODE = 42;
		public const byte CLIENT_BROADCAST_CODE = 24;

		public const byte BROADCAST_LISTING_REQUEST_1 = 42;
		public const byte BROADCAST_LISTING_REQUEST_2 = 24;
		public const byte BROADCAST_LISTING_REQUEST_3 = 9;

		public const ushort DEFAULT_PORT = 15937;

		private static List<CachedUdpClient> localListingsClientList = new List<CachedUdpClient>();

		public static IPEndPoint ResolveHost(string host, ushort port)
		{
			// Check for any localhost type addresses
			if (host == "0.0.0.0" || host == "127.0.0.1" || host == "::0")
				return new IPEndPoint(IPAddress.Parse(host), port);
			else if (host == "localhost")
				return new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

			IPAddress ipAddress;

			if (!IPAddress.TryParse(host, out ipAddress))
			{
				IPHostEntry hostCheck = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in hostCheck.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						if (ip.ToString() == host)
							return new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
					}
				}

				try
				{
					IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
					ipAddress = ipHostInfo.AddressList[0];
				}
				catch
				{
					Logging.BMSLog.Log("Failed to find host");
					throw new ArgumentException("Unable to resolve host");
				}
			}

			return new IPEndPoint(ipAddress, port);
		}

		public struct BroadcastEndpoints
		{
			public string Address { get; private set; }
			public ushort Port { get; private set; }
			public bool IsServer { get; private set; }

			public BroadcastEndpoints(string address, ushort port, bool isServer) : this()
			{
				this.Address = address;
				this.Port = port;
				this.IsServer = isServer;
			}
		}

		public static List<BroadcastEndpoints> LocalEndpoints { get; private set; }

		public static bool EndingSession { get; private set; }

		#region Delegates
		/// <summary>
		/// A base delegate for any kind of network event
		/// </summary>
		public delegate void BaseNetworkEvent(NetWorker sender);

		/// <summary>
		/// Used to fire events that relate to a broadcast endpoint
		/// </summary>
		public delegate void BroadcastEndpointEvent(BroadcastEndpoints endpoint, NetWorker sender);

		/// <summary>
		/// A base delegate for any kind of network ping event
		/// </summary>
		/// <param name="ping">The latency between client and server</param>
		public delegate void PingEvent(double ping, NetWorker sender);

		/// <summary>
		/// Used for any events that relate to a NetworkingPlayer
		/// </summary>
		public delegate void PlayerEvent(NetworkingPlayer player, NetWorker sender);

		/// <summary>
		/// Used for any events that relate to a frame and the target player
		/// </summary>
		/// <param name="player">The player the message came from</param>
		/// <param name="frame">The frame data</param>
		public delegate void FrameEvent(NetworkingPlayer player, FrameStream frame, NetWorker sender);

		/// <summary>
		/// Used for any events that relate to a binary frame and the target player
		/// </summary>
		/// <param name="player">The player the message came from</param>
		/// <param name="frame">The frame data</param>
		public delegate void BinaryFrameEvent(NetworkingPlayer player, Binary frame, NetWorker sender);

		/// <summary>
		/// Used for any events that relate to a text frame and the target player
		/// </summary>
		/// <param name="player">The player the message came from</param>
		/// <param name="frame">The frame data</param>
		public delegate void TextFrameEvent(NetworkingPlayer player, Text frame, NetWorker sender);
		#endregion

		#region Events
		/// <summary>
		/// Occurs when a local server has been located by calling the static SetupLocalUdpListings method
		/// </summary>
		public static event BroadcastEndpointEvent localServerLocated;

		/// <summary>
		/// Occurs when tcp listener has successfully bound
		/// </summary>
		public event BaseNetworkEvent bindSuccessful;

		/// <summary>
		/// Occurs when tcp listener has failed to bind
		/// </summary>
		public event BaseNetworkEvent bindFailure;

		/// <summary>
		/// Occurs when the server has accepted this client
		/// </summary>
		public event BaseNetworkEvent serverAccepted;

		/// <summary>
		/// Occurs when the current socket has completely disconnected
		/// </summary>
		public event BaseNetworkEvent disconnected;

		/// <summary>
		/// Occurs when the current socket was forcibly disconnected
		/// </summary>
		public event BaseNetworkEvent forcedDisconnect;

		/// <summary>
		/// Occurs when a player has connected
		/// </summary>
		public event PlayerEvent playerConnected;

		/// <summary>
		/// Occurs when a player has disconnected
		/// </summary>
		public event PlayerEvent playerDisconnected;

		/// <summary>
		/// Occurs when a player has timed out
		/// </summary>
		public event PlayerEvent playerTimeout;

		/// <summary>
		/// Occurs when the player has connected and been validated by the server
		/// </summary>
		public event PlayerEvent playerAccepted;

		/// <summary>
		/// Occurs when the player has connected and was not able to be validated by the server
		/// </summary>
		public event PlayerEvent playerRejected;

        /// <summary>
        /// Occurs when the player has connected and been succesfully authenticated
        /// </summary>
        public event PlayerEvent playerAuthenticated;

        /// <summary>
        /// Occurs when a message is received over the network from a remote machine
        /// </summary>
        public event FrameEvent messageReceived;

		/// <summary>
		/// Occurs when a binary message is received over the network from a remote machine
		/// </summary>
		public event BinaryFrameEvent binaryMessageReceived;

		/// <summary>
		/// Occurs when a binary message is received and its router byte is the byte for Rpc
		/// </summary>
		//public event BinaryFrameEvent rpcMessageReceived;

		/// <summary>
		/// Occurs when a text message is received over the network from a remote machine
		/// </summary>
		public event TextFrameEvent textMessageReceived;

		/// <summary>
		/// Occurs when a ping is received over the network from a remote machine
		/// </summary>
		public event PingEvent onPingPong;

		/// <summary>
		/// Called when a player has provided it's guid, this is useful for waiting until
		/// the player is uniquely identifiable across networkers
		/// </summary>
		public event PlayerEvent playerGuidAssigned;

		/// <summary>
		/// Occurs when a client get's an id from the server asynchronously that belongs to this NetworkObject
		/// </summary>
		public event NetworkObject.CreateEvent objectCreateAttach;



		/// <summary>
		/// Occurs when a network object has been created on the network
		/// </summary>
		public event NetworkObject.NetworkObjectEvent objectCreated {
			add {
				if (_objectCreated == null || !_objectCreated.GetInvocationList().Contains(value))
					_objectCreated += value;
			}

			remove {
				_objectCreated -= value;
			}
		}
		private NetworkObject.NetworkObjectEvent _objectCreated;

		/// <summary>
		/// TODO: COMMENT
		/// </summary>
		public event NetworkObject.CreateRequestEvent objectCreateRequested;

		/// <summary>
		/// TODO: COMMENT
		/// </summary>
		public event NetworkObject.NetworkObjectEvent factoryObjectCreated;
		#endregion

		#region Properties
		/// <summary>
		/// The list of all of the networked players. This is a wrapper around the native network
		/// socket with extra meta-data for each connection
		/// </summary>
		public List<NetworkingPlayer> Players { get; private set; }

		/// <summary>
		/// A list of all of the players that are to be disconnected. This is useful if a player needs
		/// to disconnect while they are currently locked
		/// </summary>
		protected List<NetworkingPlayer> DisconnectingPlayers { get; private set; }

		/// <summary>
		/// A list of all of the players that are to be forcibly disconnected.
		/// This is useful if a player needs to disconnect while they are currently locked
		/// </summary>
		protected List<NetworkingPlayer> ForcedDisconnectingPlayers { get; private set; }

		/// <summary>
		/// Represents the maximum allowed connections to this listener
		/// </summary>
		/// <value>Gets and sets the max allowed connections connections</value>
		public int MaxConnections { get; private set; }

		/// <summary>
		/// This is a count for every player that has successfully connected since the start of this server,
		/// this also serves to be the unique id for this connection
		/// </summary>
		/// <value>The current count of players on the network</value>
		public uint ServerPlayerCounter { get; protected set; }

		/// <summary>
		/// The port for this networker
		/// </summary>
		public ushort Port { get; private set; }

		/// <summary>
		/// A helper to determine if this NetWorker is a server
		/// </summary>
		public bool IsServer { get { return this is IServer; } }

		/// <summary>
		/// A handle to the server cache to make cache requests
		/// </summary>
		public Cache ServerCache { get; private set; }

		/// <summary>
		/// Used to determine how much bandwidth (in bytes) hass been read
		/// </summary>
		public ulong BandwidthIn { get; protected set; }

		/// <summary>
		/// Used to determine how much bandwidth (in bytes) hass been written
		/// </summary>
		public ulong BandwidthOut { get; set; }

		/// <summary>
		/// Used to simulate packet loss, should be a number between 0.0f and 1.0f (percentage)
		/// </summary>
		public float PacketLossSimulation { get; set; }

		/// <summary>
		/// Used to simulate network latency to test experience at high pings
		/// </summary>
		public int LatencySimulation { get; set; }

		internal bool ObjectCreatedRegistered { get { return _objectCreated != null; } }

		/// <summary>
		/// A cached BMSByte to prevent large amounts of garbage collection on packet sequences
		/// </summary>
		public BMSByte PacketSequenceData { get; private set; }
		#endregion

		/// <summary>
		/// The distance from the proximity location in order to receive proximity
		/// messages from other players
		/// </summary>
		public float ProximityDistance { get; set; }

        /// <summary>
        /// How often a client gets updates if it is outside the proximity range.
		/// 
		/// If set to 0 clients that are too far will never get updated.
        /// </summary>
        public int ProximityModeUpdateFrequency { get; set; }

        /// <summary>
        /// Allows the newly created network object to be queued for the flush call
        /// </summary>
        public bool PendCreates { get; set; }

		/// <summary>
		/// A boolean to tell the read thread to stop reading and close
		/// </summary>
		protected bool readThreadCancel = false;

		/// <summary>
		/// A player reference to the current machine
		/// </summary>
		public NetworkingPlayer Me { get; protected set; }

		public Dictionary<uint, List<Action<NetworkObject>>> missingObjectBuffer = new Dictionary<uint, List<Action<NetworkObject>>>();

		/// <summary>
		/// Determine whether the socket is connected
		/// </summary>
		public bool IsConnected
		{
			get
			{
				if (Me != null)
					return Me.Connected;

				return false;
			}
		}

		/// <summary>
		/// A dictionary of all of the network objects indexed by it's id
		/// </summary>
		public Dictionary<uint, NetworkObject> NetworkObjects { get; private set; }

		/// <summary>
		/// A list of all of the network objects
		/// </summary>
		public List<NetworkObject> NetworkObjectList { get; private set; }

		/// <summary>
		/// Used to give a unique id to each of the network objects that are added
		/// </summary>
		private uint currentNetworkObjectId = 0;

		/// <summary>
		/// This object is to track the time for this networker which is also known
		/// as a "time step" in this system
		/// </summary>
		public TimeManager Time { get; set; }

		/// <summary>
		/// Used to determine if this networker has been bound yet
		/// </summary>
		public bool IsBound { get; private set; }

		/// <summary>
		/// Used to determine if this NetWorker has already been disposed to avoid re-connections
		/// </summary>
		public bool Disposed { get; private set; }

		/// <summary>
		/// The unique GUID that will represent all networkers for this process instance
		/// </summary>
		public static Guid InstanceGuid { get; private set; }
		private static bool setupInstanceGuid = false;

        /// <summary>
        /// Used to authenticate the client/server connection. If null, does not perform authentication.
        /// </summary>
        protected IUserAuthenticator authenticator = null;

        /// <summary>
        /// This is the base constructor which is normally used for clients and not classes
        /// acting as hosts
        /// </summary>
        public NetWorker()
		{
			Initialize();
		}

		/// <summary>
		/// Constructor with a given Maximum allowed connections
		/// </summary>
		/// <param name="maxConnections">The Maximum connections allowed</param>
		public NetWorker(int maxConnections)
		{
			Initialize();
			MaxConnections = maxConnections;
		}

		/// <summary>
		/// Used to setup any variables and private set properties, time and other 
		/// network critical variables that relate to a worker
		/// </summary>
		private void Initialize()
		{
			PacketSequenceData = new BMSByte();

			if (!setupInstanceGuid)
			{
				InstanceGuid = Guid.NewGuid();
				setupInstanceGuid = true;
			}

			// Setup the time if it hasn't been assigned already
			Time = new TimeManager();

			Players = new List<NetworkingPlayer>();
			DisconnectingPlayers = new List<NetworkingPlayer>();
			ForcedDisconnectingPlayers = new List<NetworkingPlayer>();
			NetworkObjects = new Dictionary<uint, NetworkObject>();
			NetworkObjectList = new List<NetworkObject>();

			ServerPlayerCounter = 0;

			ServerCache = new Cache(this);
			EndingSession = false;
		}

		/// <summary>
		/// Called once the network connection has been bound
		/// </summary>
		protected virtual void NetworkInitialize()
		{
			Task.Queue(() =>
			{
				while (IsBound)
				{
					ulong step = Time.Timestep;
					lock (NetworkObjects)
					{
						foreach (NetworkObject obj in NetworkObjects.Values)
						{
							// Only do the heartbeat (update) on network objects that
							// are owned by the current networker
							if ((obj.IsOwner && obj.UpdateInterval > 0) || (IsServer && obj.AuthorityUpdateMode))
								obj.HeartBeat(step);
						}
					}

					Thread.Sleep(10);
				}
			});
		}

		public void CompleteInitialization(NetworkObject networkObject)
		{
			lock (NetworkObjects)
			{
				if (NetworkObjects.ContainsKey(networkObject.NetworkId))
					return;

				NetworkObjects.Add(networkObject.NetworkId, networkObject);
				NetworkObjectList.Add(networkObject);
			}
		}

		public void FlushCreateActions(NetworkObject networkObject)
		{
			List<Action<NetworkObject>> actions = null;
			lock (missingObjectBuffer)
			{
				missingObjectBuffer.TryGetValue(networkObject.NetworkId, out actions);
				missingObjectBuffer.Remove(networkObject.NetworkId);
			}

			if (actions == null)
				return;

			foreach (var action in actions)
				action(networkObject);
		}

        /// <summary>
        /// Iterate over all NetworkingPlayers in a thread-safe manner
        /// </summary>
        public void IteratePlayers(Action<NetworkingPlayer> expression)
		{
			lock (Players)
			{
				for (int i = 0; i < Players.Count; i++)
					expression(Players[i]);
			}
		}

        /// <summary>
        /// Iterate over all NetworkObjects in a thread-safe manner
        /// </summary>
        public void IterateNetworkObjects(Action<NetworkObject> expression)
		{
			lock (NetworkObjectList)
			{
				for (int i = 0; i < NetworkObjectList.Count; i++)
					expression(NetworkObjectList[i]);
			}
		}

        /// <summary>
        /// Retrieve a NetworkingPlayer by NetworkId
        /// </summary>
        /// <returns>
        /// The NetworkingPlayer with NetworkId equal to id
        /// </returns>
        /// <param name="id">The NetworkId of the NetworkingPlayer</param>
        public NetworkingPlayer GetPlayerById(uint id)
		{
			lock (Players)
			{
				for (int i = 0; i < Players.Count; i++)
				{
					if (Players[i].NetworkId == id)
						return Players[i];
				}
			}

			return null;
		}

        /// <summary>
        /// Retrieve a NetworkingPlayer based on an expression
        /// </summary>
        /// <returns>
        /// The first NetworkingPlayer to match the provided expression, or null if no matches are found
        /// </returns>
        /// <param name="expression">The expression on which to match</param>
        public NetworkingPlayer FindPlayer(Func<NetworkingPlayer, bool> expression)
		{
			lock (Players)
			{
				return Players.FirstOrDefault(expression);
			}
		}

        /// <summary>
        /// Retrieve a NetworkingPlayer which matches the provided NetworkingPlayer by Ip and InstanceGuid
        /// </summary>
        /// <returns>
        /// The first NetworkingPlayer to match the provided expression, or null if no matches are found
        /// </returns>
        /// <param name="expression">The expression on which to match</param>
        public NetworkingPlayer FindMatchingPlayer(NetworkingPlayer other)
		{
			if (other.Networker == this)
				return other;

			lock (Players)
			{
				for (int i = 0; i < Players.Count; i++)
				{
					if (Players[i].Ip == other.Ip && Players[i].InstanceGuid == other.InstanceGuid)
						return Players[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Register a networked object with this networker
		/// </summary>
		/// <param name="networkObject">The object that is to be registered with this networker</param>
		/// <returns><c>true</c> if the object was registered successfully, else <c>false</c> if it has already been registered</returns>
		public bool RegisterNetworkObject(NetworkObject networkObject, uint forceId = 0)
		{
			uint id = currentNetworkObjectId;

			lock (NetworkObjects)
			{
				// If we are forcing this object
				if (forceId != 0)
				{
					if (NetworkObjects.ContainsKey(forceId))
						return false;

					id = forceId;

					if (!networkObject.RegisterOnce(id))
						throw new BaseNetworkException("The supplied network object has already been assigned to a networker and has an id");

					//AddNetworkObject(forceId, networkObject);
					//NetworkObjectList.Add(networkObject);
				}
				else
				{
					do
					{
						if (NetworkObjects.ContainsKey(++currentNetworkObjectId))
							continue;

						if (!networkObject.RegisterOnce(currentNetworkObjectId))
						{
							// Backtrack since the next call to this method will increment before checking
							currentNetworkObjectId--;

							throw new BaseNetworkException("The supplied network object has already been assigned to a networker and has an id");
						}

						//AddNetworkObject(currentNetworkObjectId, networkObject);
						//NetworkObjectList.Add(networkObject);
						break;
					} while (IsBound);
				}
			}

			// Assign the network id to the network object
			networkObject.RegisterOnce(id);

			// When this object is destroyed it needs to remove itself from the list
			networkObject.onDestroy += (NetWorker sender) =>
			{
				lock (NetworkObjects)
				{
					NetworkObjects.Remove(networkObject.NetworkId);
					NetworkObjectList.Remove(networkObject);
				}
			};

			return true;
		}

		/// <summary>
		/// Disconnect this client from the server
		/// </summary>
		/// <param name="forced">Used to tell if this disconnect was intentional <c>false</c> or caused by an exception <c>true</c></param>
		public abstract void Disconnect(bool forced);

		/// <summary>
		/// Reads the frame stream as if it were read on the network
		/// </summary>
		/// <param name="frame">The target frame stream to be read</param>
		public abstract void FireRead(FrameStream frame, NetworkingPlayer currentPlayer);

		/// <summary>
		/// Goes through all of the pending disconnect players and disconnects them
		/// Pending disconnects are always forced
		/// </summary>
		protected void DisconnectPending(Action<NetworkingPlayer, bool> disconnectMethod)
		{
			if (DisconnectingPlayers.Count == 0 && ForcedDisconnectingPlayers.Count == 0)
				return;

			lock (Players)
			{
				for (int i = DisconnectingPlayers.Count - 1; i >= 0; --i)
					disconnectMethod(DisconnectingPlayers[i], false);

				for (int i = ForcedDisconnectingPlayers.Count - 1; i >= 0; --i)
					disconnectMethod(ForcedDisconnectingPlayers[i], true);
			}
		}

		/// <summary>
		/// A wrapper for the bindSuccessful event call that chindren of this calls can call
		/// </summary>
		protected void OnBindSuccessful()
		{
			IsBound = true;
			NetworkInitialize();
			if (bindSuccessful != null)
				bindSuccessful(this);
		}

		/// <summary>
		/// A wrapper for the bindFailure event call that children of this can call
		/// </summary>
		protected void OnBindFailure()
		{
			if (bindFailure != null)
				bindFailure(this);
		}

		/// <summary>
		/// A wrapper for the playerDisconnected event call that chindren of this can call.
		/// This also is responsible for adding the player to the lookup
		/// </summary>
		protected void OnPlayerConnected(NetworkingPlayer player)
		{
			if (Players.Contains(player))
				throw new BaseNetworkException("Cannot add player because it already exists in the list");

			// Removal of clients can be from any thread
			lock (Players)
			{
				Players.Add(player);
			}

			if (playerConnected != null)
				playerConnected(player, this);
		}

		internal void OnObjectCreated(NetworkObject target)
		{
			if (_objectCreated != null)
                _objectCreated(target);
		}

		internal void OnObjectCreateAttach(int identity, int hash, uint id, FrameStream frame)
		{
			if (objectCreateAttach != null)
				objectCreateAttach(identity, hash, id, frame);
		}

		internal void OnObjectCreateRequested(int identity, uint id, FrameStream frame, Action<NetworkObject> callback)
		{
			if (objectCreateRequested != null)
				objectCreateRequested(this, identity, id, frame, callback);
		}

		internal void OnFactoryObjectCreated(NetworkObject obj)
		{
			if (factoryObjectCreated != null)
				factoryObjectCreated(obj);
		}

		/// <summary>
		/// A wrapper for the bindFailure event call that chindren of this can call.
		/// This also is responsible for removing the player from the lookup
		/// </summary>
		protected void OnPlayerDisconnected(NetworkingPlayer player)
		{
			// Removal of clients can be from any thread
			lock (Players)
			{
				Players.Remove(player);
			}

			player.OnDisconnect();

			if (playerDisconnected != null)
				playerDisconnected(player, this);
		}

		protected void OnPlayerTimeout(NetworkingPlayer player)
		{
			if (playerTimeout != null)
				playerTimeout(player, this);
		}

		/// <summary>
		/// A wrapper for the playerAccepted event call that chindren of this can call
		/// </summary>
		protected void OnPlayerAccepted(NetworkingPlayer player)
		{
			player.Accepted = true;
			player.PendingAccepted = false;

			NetworkObject[] currentObjects;
			lock (NetworkObjects)
			{
				currentObjects = NetworkObjects.Values.ToArray();
			}

			NetworkObject.PlayerAccepted(player, currentObjects);

			if (playerAccepted != null)
				playerAccepted(player, this);
		}

		/// <summary>
		/// A wrapper for the playerAccepted event call that chindren of this can call
		/// </summary>
		protected void OnPlayerRejected(NetworkingPlayer player)
		{
			player.Accepted = false;
            player.Authenticated = false;

			if (playerRejected != null)
				playerRejected(player, this);
		}

        /// <summary>
        /// If the player is authenticated, 
        /// </summary>
        protected void OnPlayerAuthenticated(NetworkingPlayer player)
        {
            player.Authenticated = true;

            if (playerAuthenticated != null)
                playerAuthenticated(player, this);
        }

		/// <summary>
		/// Set the port for the networker
		/// </summary>
		/// <param name="port"></param>
		protected void SetPort(ushort port)
		{
			this.Port = port;
		}

		/// <summary>
		/// A wrapper for the pingReceived event call that children of this can call
		/// </summary>
		/// <param name="ping"></param>
		protected void OnPingRecieved(double ping, NetworkingPlayer player)
		{
			if (onPingPong != null)
				onPingPong(ping, this);

			player.RoundTripLatency = (int)ping;
		}

		/// <summary>
		/// A wrapper for the messageReceived event call that chindren of this can call
		/// </summary>
		protected void OnMessageReceived(NetworkingPlayer player, FrameStream frame)
		{
			if (frame.GroupId == MessageGroupIds.NETWORK_ID_REQUEST && this is IClient)
			{
				Time.SetStartTime(frame.TimeStep);
				Me = new NetworkingPlayer(frame.StreamData.GetBasicType<uint>(), "0.0.0.0", false, null, this);
				Me.AssignPort(Port);
				OnServerAccepted();
				return;
			}

			if (frame.GroupId == MessageGroupIds.PING || frame.GroupId == MessageGroupIds.PONG)
			{
				long receivedTimestep = frame.StreamData.GetBasicType<long>();
				DateTime received = new DateTime(receivedTimestep);
				TimeSpan ms = DateTime.UtcNow - received;

				if (frame.GroupId == MessageGroupIds.PING)
					Pong(player, received);
				else
					OnPingRecieved(ms.TotalMilliseconds, player);

				return;
			}

			if (frame is Binary)
			{
				byte routerId = ((Binary)frame).RouterId;
				if (routerId == RouterIds.RPC_ROUTER_ID || routerId == RouterIds.BINARY_DATA_ROUTER_ID || routerId == RouterIds.CREATED_OBJECT_ROUTER_ID)
				{
					uint id = frame.StreamData.GetBasicType<uint>();
					NetworkObject targetObject = null;

					lock (NetworkObjects)
					{
						NetworkObjects.TryGetValue(id, out targetObject);
					}

					if (targetObject == null)
					{
						lock (missingObjectBuffer)
						{
							if (!missingObjectBuffer.ContainsKey(id))
								missingObjectBuffer.Add(id, new List<Action<NetworkObject>>());

							missingObjectBuffer[id].Add((networkObject) =>
							{
								ExecuteRouterAction(routerId, networkObject, (Binary)frame, player);
							});
						}

						// TODO:  If the server is missing an object, it should have a timed buffer
						// that way useless messages are not setting around in memory

						return;
					}

					ExecuteRouterAction(routerId, targetObject, (Binary)frame, player);
				}
				else if (routerId == RouterIds.NETWORK_OBJECT_ROUTER_ID)
				{
					NetworkObject.CreateNetworkObject(this, player, (Binary)frame);
				}
				else if (routerId == RouterIds.ACCEPT_MULTI_ROUTER_ID)
					NetworkObject.CreateMultiNetworkObject(this, player, (Binary)frame);
				else if (binaryMessageReceived != null)
					binaryMessageReceived(player, (Binary)frame, this);
			}
			else if (frame is Text && textMessageReceived != null)
				textMessageReceived(player, (Text)frame, this);

			if (messageReceived != null)
				messageReceived(player, frame, this);
		}

		private void ExecuteRouterAction(byte routerId, NetworkObject networkObject, Binary frame, NetworkingPlayer player)
		{
			if (routerId == RouterIds.RPC_ROUTER_ID)
				networkObject.InvokeRpc(player, frame.TimeStep, frame.StreamData, frame.Receivers);
			else if (routerId == RouterIds.BINARY_DATA_ROUTER_ID)
				networkObject.ReadBinaryData(frame);
			else if (routerId == RouterIds.CREATED_OBJECT_ROUTER_ID)
				networkObject.CreateConfirmed(player);
		}

		/// <summary>
		/// When this socket has been disconnected
		/// </summary>
		protected void OnDisconnected()
		{
			IsBound = false;

			if (Me != null)
			{
				Me.Connected = false;

				if (!(this is IServer))
					Me.OnDisconnect();
			}

			if (disconnected != null)
				disconnected(this);

			Disposed = true;
		}

		/// <summary>
		/// When this socket has been forcibly disconnected
		/// </summary>
		protected void OnForcedDisconnect()
		{
			IsBound = false;

			if (forcedDisconnect != null)
				forcedDisconnect(this);

			Disposed = true;
		}

		/// <summary>
		/// A wrapper around calling the serverAccepted event from child classes
		/// </summary>
		protected void OnServerAccepted()
		{
			Me.Connected = true;

			if (serverAccepted != null)
				serverAccepted(this);
		}

		/// <summary>
		/// A wrapper around calling the playerGuidAssigned event from child classes
		/// </summary>
		/// <param name="player">The player which the guid was assigned to</param>
		protected void OnPlayerGuidAssigned(NetworkingPlayer player)
		{
			if (playerGuidAssigned != null)
				playerGuidAssigned(player, this);
		}

		/// <summary>
		/// A wrapper around calling the playerGuidAssigned event from child classes
		/// </summary>
		/// <param name="player">The player which the guid was assigned to</param>
		/// <param name="rejected">Returns whether the player was rejected during the handling of the event</param>
		protected void OnPlayerGuidAssigned(NetworkingPlayer player, out bool rejected)
		{
			OnPlayerGuidAssigned(player);

			// Return if the player was rejected during the handling of the event.
			rejected = (player.IsDisconnecting || DisconnectingPlayers.Contains(player) || ForcedDisconnectingPlayers.Contains(player));
		}

        /// <summary>
        /// Used to set the user authenticator. NetWorker must not already be connected.
        /// </summary>
        public void SetUserAuthenticator(IUserAuthenticator authenticator)
        {
            if (IsConnected)
                throw new BaseNetworkException("The NetWorker is already connected");

            this.authenticator = authenticator;
        }

        /// <summary>
        /// Used to bind to a port then unbind to trigger any operating system firewall requests
        /// </summary>
        public static void PingForFirewall(ushort port = 0)
		{
			if (port < 1)
			{
				port = DEFAULT_PORT - 1;
			}
			Task.Queue(() =>
			{
				try
				{
					//IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
					//IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 15937);
					IPEndPoint ipLocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

					TcpListener t = new TcpListener(ipLocalEndPoint);
					t.Start();
					t.Stop();
				}
				catch (Exception ex)
				{
					Logging.BMSLog.LogException(ex);
				}
			});
		}

		public static void EndSession()
		{
			EndingSession = true;
			CloseLocalListingsClient();

			// Reset the ending session after 1000ms so that we know all the threads have cleaned up
			// for any remaining threads that may be going for this previous process
			Task.Queue(() =>
			{
				EndingSession = false;
			}, 1000);
		}

		public Ping GeneratePing()
		{
			BMSByte payload = new BMSByte();
			long ticks = DateTime.UtcNow.Ticks;
			payload.BlockCopy<long>(ticks, sizeof(long));
			return new Ping(Time.Timestep, this is TCPClient, payload, Receivers.Server, MessageGroupIds.PING, this is BaseTCP);
		}

		protected Pong GeneratePong(DateTime time)
		{
			BMSByte payload = new BMSByte();
			long ticks = time.Ticks;
			payload.BlockCopy<long>(ticks, sizeof(long));
			return new Pong(Time.Timestep, this is TCPClient, payload, Receivers.Target, MessageGroupIds.PONG, this is BaseTCP);
		}

		/// <summary>
		/// Request the ping from the server (pingReceived will be triggered if it receives it)
		/// This is not a reliable call in UDP
		/// </summary>
		public abstract void Ping();

		protected abstract void Pong(NetworkingPlayer playerRequesting, DateTime time);

		private static void CloseLocalListingsClient()
		{
			lock (localListingsClientList) {
				foreach (CachedUdpClient cachedUdpClient in localListingsClientList) {
					cachedUdpClient.Client.Close();
				}
				localListingsClientList.Clear();
			}
		}

		/// <summary>
		/// Collects all local IPs of every NIC that is currently in operational status <c>Up</c> (active).
		/// Note: Only NICs of type <c>Wireless80211</c> and <c>Ethernet</c> are considered.
		/// </summary>
		/// <returns>An array of local IPs for every active NIC</returns>
		private static IPAddress[] GetLocalIPs() {
			List<IPAddress> ipList = new List<IPAddress>();

			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
				switch (nic.NetworkInterfaceType) {
					case NetworkInterfaceType.Wireless80211:
					case NetworkInterfaceType.Ethernet:
						break;
					default:
						continue;
				}

				if (nic.OperationalStatus != OperationalStatus.Up) continue;

				foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses) {
					if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
						ipList.Add(ip.Address);
					}
				}
			}

			return ipList.ToArray();
		}

		/// <summary>
		/// A method to find all of the local UDP servers and clients on the network
		/// </summary>
		public static void RefreshLocalUdpListings(ushort portNumber = DEFAULT_PORT, int responseBuffer = 1000)
		{
			lock (localListingsClientList) {
				foreach (CachedUdpClient cachedUdpClient in localListingsClientList) {
					cachedUdpClient.Client.Close();
				}
				localListingsClientList.Clear();
			}

			// Initialize the list to hold all of the local network endpoints that respond to the request
			if (LocalEndpoints == null)
				LocalEndpoints = new List<BroadcastEndpoints>();

			// Make sure to clear out the existing endpoints
			lock (LocalEndpoints)
			{
				LocalEndpoints.Clear();
			}

			foreach (IPAddress ipAddress in GetLocalIPs())
			{
				// Create a client to write on the network and discover other clients and servers
				CachedUdpClient localListingsClient = new CachedUdpClient(new IPEndPoint(ipAddress, 19375));
				localListingsClient.EnableBroadcast = true;
				lock (localListingsClientList) {
					localListingsClientList.Add(localListingsClient);
				}
				Task.Queue(() => { CloseLocalListingsClient(); }, responseBuffer);

				Task.Queue(() =>
				{
					IPEndPoint groupEp = default(IPEndPoint);
					string endpoint = string.Empty;

					localListingsClient.Send(new byte[] {BROADCAST_LISTING_REQUEST_1, BROADCAST_LISTING_REQUEST_2, BROADCAST_LISTING_REQUEST_3}, 3,
						new IPEndPoint(IPAddress.Parse("255.255.255.255"), portNumber));

					try
					{
						while (localListingsClient != null && !EndingSession)
						{
							var data = localListingsClient.Receive(ref groupEp, ref endpoint);

							if (data.Size != 1)
								continue;

							string[] parts = endpoint.Split('+');
							string address = parts[0];
							ushort port = ushort.Parse(parts[1]);
							if (data[0] == SERVER_BROADCAST_CODE)
							{
								var ep = new BroadcastEndpoints(address, port, true);
								LocalEndpoints.Add(ep);

								if (localServerLocated != null)
									localServerLocated(ep, null);
							} else if (data[0] == CLIENT_BROADCAST_CODE)
								LocalEndpoints.Add(new BroadcastEndpoints(address, port, false));
						}
					} catch
					{ }
				});
			}
		}
	}
}

using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public partial class NetworkManager : MonoBehaviour
	{
		public static NetworkManager Instance { get; private set; }

		public UnityAction<int, LoadSceneMode> networkSceneChanging;
		public UnityAction<Scene, LoadSceneMode> networkSceneLoaded;
		public event NetWorker.PlayerEvent playerLoadedScene;

		public NetWorker Networker { get; private set; }
		public NetWorker MasterServerNetworker { get; private set; }
		public Dictionary<int, INetworkBehavior> pendingObjects = new Dictionary<int, INetworkBehavior>();
		public Dictionary<int, NetworkObject> pendingNetworkObjects = new Dictionary<int, NetworkObject>();

		private List<int> loadedScenes = new List<int>();

		public bool IsServer { get { return Networker.IsServer; } }

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			MainThreadManager.Create();

			// This object should move through scenes
			DontDestroyOnLoad(gameObject);

			NetworkObject.objectCreated += CreatePendingObjects;
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		}

		public void Initialize(NetWorker networker, string masterServerHost = "", ushort masterServerPort = 15940, JSONNode masterServerRegisterData = null)
		{
			Networker = networker;
			Networker.binaryMessageReceived += ReadBinary;

			UnityObjectMapper.Instance.UseAsDefault();
			NetworkObject.Factory = new NetworkObjectFactory();

			if (Networker is IServer)
			{
				if (!string.IsNullOrEmpty(masterServerHost))
					RegisterOnMasterServer(networker.Port, networker.MaxConnections, masterServerHost, masterServerPort, masterServerRegisterData);

				Networker.playerAccepted += PlayerAcceptedSceneSetup;
			}
		}

		private void CreatePendingObjects(NetworkObject obj)
		{
			INetworkBehavior behavior;

			if (!pendingObjects.TryGetValue(obj.CreateCode, out behavior))
			{
				if (obj.CreateCode < 0)
					pendingNetworkObjects.Add(obj.CreateCode, obj);

				return;
			}

			behavior.Initialize(obj);
			pendingObjects.Remove(obj.CreateCode);

			//if (pendingObjects.Count == 0)
			//	NetworkObject.objectCreated -= CreatePendingObjects;
		}

		public void MatchmakingServersFromMasterServer(string masterServerHost,
			ushort masterServerPort,
			int elo,
			System.Action<MasterServerResponse> callback = null,
			string gameId = "myGame",
			string gameType = "any",
			string gameMode = "all")
		{
			// The Master Server communicates over TCP
			TCPMasterClient client = new TCPMasterClient();

			// Once this client has been accepted by the master server it should send it's get request
			client.serverAccepted += () =>
			{
				try
				{
					// Create the get request with the desired filters
					JSONNode sendData = JSONNode.Parse("{}");
					JSONClass getData = new JSONClass();
					getData.Add("id", gameId);
					getData.Add("type", gameType);
					getData.Add("mode", gameMode);
					getData.Add("elo", new JSONData(elo));

					sendData.Add("get", getData);

					// Send the request to the server
					client.Send(Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
				}
				catch
				{
					// If anything fails, then this client needs to be disconnected
					client.Disconnect(true);
					client = null;

					MainThreadManager.Run(() =>
					{
						if (callback != null)
							callback(null);
					});
				}
			};

			// An event that is raised when the server responds with hosts
			client.textMessageReceived += (player, frame) =>
			{
				try
				{
					// Get the list of hosts to iterate through from the frame payload
					JSONNode data = JSONNode.Parse(frame.ToString());
					MainThreadManager.Run(() =>
					{
						if (data["hosts"] != null)
						{
							MasterServerResponse response = new MasterServerResponse(data["hosts"].AsArray);
							if (callback != null)
								callback(response);
						}
						else
						{
							if (callback != null)
								callback(null);
						}
					});
				}
				finally
				{
					if (client != null)
					{
						// If we succeed or fail the client needs to disconnect from the Master Server
						client.Disconnect(true);
						client = null;
					}
				}
			};

			try
			{
				client.Connect(masterServerHost, masterServerPort);
			}
			catch (System.Exception ex)
			{
				Debug.LogError(ex.Message);
				MainThreadManager.Run(() =>
				{
					if (callback != null)
						callback(null);
				});
			}
		}

		public JSONNode MasterServerRegisterData(NetWorker server, string id, string serverName, string type, string mode, string comment = "", bool useElo = false, int eloRequired = 0)
		{
			// Create the get request with the desired filters
			JSONNode sendData = JSONNode.Parse("{}");
			JSONClass registerData = new JSONClass();
			registerData.Add("id", id);
			registerData.Add("name", serverName);
			registerData.Add("port", new JSONData(server.Port));
			registerData.Add("playerCount", new JSONData(0));
			registerData.Add("maxPlayers", new JSONData(server.MaxConnections));
			registerData.Add("comment", comment);
			registerData.Add("type", type);
			registerData.Add("mode", mode);
			registerData.Add("protocol", server is UDPServer ? "udp" : "tcp");
			registerData.Add("elo", new JSONData(eloRequired));
			registerData.Add("useElo", new JSONData(useElo));
			sendData.Add("register", registerData);

			return sendData;
		}

		private void RegisterOnMasterServer(ushort port, int maxPlayers, string masterServerHost, ushort masterServerPort, JSONNode masterServerData)
		{
			// The Master Server communicates over TCP
			TCPMasterClient client = new TCPMasterClient();

			// Once this client has been accepted by the master server it should send it's get request
			client.serverAccepted += () =>
			{
				try
				{
					Frame.Text temp = Frame.Text.CreateFromString(client.Time.Timestep, masterServerData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_REGISTER, true);

					//Debug.Log(temp.GetData().Length);
					// Send the request to the server
					client.Send(temp);
				}
				catch
				{
					// If anything fails, then this client needs to be disconnected
					client.Disconnect(true);
					client = null;
				}
			};

			client.Connect(masterServerHost, masterServerPort);

			Networker.disconnected += () =>
			{
				client.Disconnect(false);
				MasterServerNetworker = null;
			};

			MasterServerNetworker = client;
		}

		public void Disconnect()
		{
			if (Networker != null)
				Networker.Disconnect(false);

			NetWorker.EndSession();

			pendingObjects.Clear();
			pendingNetworkObjects.Clear();
			MasterServerNetworker = null;
			Networker = null;
			Instance = null;
			Destroy(gameObject);
		}

		private void OnApplicationQuit()
		{
			if (Networker != null)
				Networker.Disconnect(false);

			NetWorker.EndSession();
		}

		private void FixedUpdate()
		{
			if (Networker != null)
			{
				for (int i = 0; i < Networker.NetworkObjectList.Count; i++)
					Networker.NetworkObjectList[i].InterpolateUpdate();
			}
		}

		private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
		{
			SceneReady(scene, mode);
		}

		private void ProcessOthers(Transform obj, NetworkObject createTarget, uint idOffset, NetworkBehavior netBehavior = null)
		{
			int i;

			var components = obj.GetComponents<NetworkBehavior>().OrderBy(n => n.GetType().ToString()).ToArray();

			// Create each network object that is available
			for (i = 0; i < components.Length; i++)
			{
				if (components[i] == netBehavior)
					continue;

				var no = components[i].CreateNetworkObject(Networker, 0);

				if (Networker.IsServer)
					FinializeInitialization(obj.gameObject, components[i], no, obj.position, obj.rotation, false, true);
				else
					components[i].AwaitNetworkBind(Networker, createTarget, idOffset++);
			}

			for (i = 0; i < obj.transform.childCount; i++)
				ProcessOthers(obj.transform.GetChild(i), createTarget, idOffset);
		}

		private void FinializeInitialization(GameObject go, INetworkBehavior netBehavior, NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true, bool skipOthers = false)
		{
			if (Networker is IServer)
				InitializedObject(netBehavior, obj);
			else
				obj.pendingInitialized += InitializedObject;

			if (position != null)
			{
				if (rotation != null)
				{
					go.transform.position = position.Value;
					go.transform.rotation = rotation.Value;
				}
				else
					go.transform.position = position.Value;
			}

			if (sendTransform)
				obj.SendRpc(NetworkBehavior.RPC_SETUP_TRANSFORM, Receivers.OthersBuffered, go.transform.position, go.transform.rotation);

			if (!skipOthers)
			{
				// Go through all associated network behaviors in the hierarchy (including self) and
				// Assign their TempAttachCode for lookup later. Should use an incrementor or something
				ProcessOthers(go.transform, obj, 1, (NetworkBehavior)netBehavior);
			}
		}

		/// <summary>
		/// Called automatically when a new player is accepted and sends the player
		/// the currently loaded scene indexes for the client to load
		/// </summary>
		/// <param name="player">The player that was just accepted</param>
		private void PlayerAcceptedSceneSetup(NetworkingPlayer player)
		{
			BMSByte data = new BMSByte();
			ObjectMapper.Instance.MapBytes(data, loadedScenes.Count);

			// Go through all the loaded scene indexes and send them to the connecting player
			for (int i = 0; i < loadedScenes.Count; i++)
				ObjectMapper.Instance.MapBytes(data, loadedScenes[i]);

			Binary frame = new Binary(Networker.Time.Timestep, false, data, Receivers.Target, MessageGroupIds.VIEW_INITIALIZE, Networker is BaseTCP);

			SendFrame(Networker, frame, player);
		}

		private void ReadBinary(NetworkingPlayer player, Binary frame)
		{
			if (frame.GroupId == MessageGroupIds.VIEW_INITIALIZE)
			{
				if (Networker is IServer)
					return;

				int count = frame.StreamData.GetBasicType<int>();

				loadedScenes.Clear();
				for (int i = 0; i < count; i++)
					loadedScenes.Add(frame.StreamData.GetBasicType<int>());

				MainThreadManager.Run(() =>
				{
					SceneManager.LoadScene(loadedScenes[0], LoadSceneMode.Single);

					for (int i = 1; i < loadedScenes.Count; i++)
						SceneManager.LoadSceneAsync(loadedScenes[i], LoadSceneMode.Additive);
				});

				return;
			}

			if (frame.GroupId != MessageGroupIds.VIEW_CHANGE)
				return;

			if (Networker.IsServer)
			{
				// The client has loaded the scene
				if (playerLoadedScene != null)
					playerLoadedScene(player);

				return;
			}

			// We need to halt the creation of network objects until we load the scene
			Networker.PendCreates = true;

			// Get the scene index that the server loaded
			int sceneIndex = frame.StreamData.GetBasicType<int>();

			// Get the mode in which the server loaded the scene
			int modeIndex = frame.StreamData.GetBasicType<int>();

			// Convert the int mode to the enum mode
			LoadSceneMode mode = (LoadSceneMode)modeIndex;

			if (networkSceneChanging != null)
				networkSceneChanging(sceneIndex, mode);

			MainThreadManager.Run(() =>
			{
				// Load the scene that the server loaded in the same LoadSceneMode
				if (mode == LoadSceneMode.Additive)
					SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
				else if (mode == LoadSceneMode.Single)
					SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
			});
		}

		/// <summary>
		/// A wrapper around the various raw send methods for the client and server types
		/// </summary>
		/// <param name="networker">The networker that is going to be sending the data</param>
		/// <param name="frame">The frame that is to be sent across the network</param>
		/// <param name="targetPlayer">The player to send the frame to, if null then will send to all</param>
		public static void SendFrame(NetWorker networker, FrameStream frame, NetworkingPlayer targetPlayer = null)
		{
			if (networker is IServer)
			{
				if (targetPlayer != null)
				{
					if (networker is TCPServer)
						((TCPServer)networker).SendToPlayer(frame, targetPlayer);
					else
						((UDPServer)networker).Send(targetPlayer, frame, true);
				}
				else
				{
					if (networker is TCPServer)
						((TCPServer)networker).SendAll(frame);
					else
						((UDPServer)networker).Send(frame, true);
				}
			}
			else
			{
				if (networker is TCPClientBase)
					((TCPClientBase)networker).Send(frame);
				else
					((UDPClient)networker).Send(frame, true);
			}
		}

		private void SceneReady(Scene scene, LoadSceneMode mode)
		{
			// If we are loading a completely new scene then we will need
			// to clear out all the old objects that were stored as they
			// are no longer needed
			if (mode != LoadSceneMode.Additive)
			{
				pendingObjects.Clear();
				pendingNetworkObjects.Clear();
				loadedScenes.Clear();
			}

			loadedScenes.Add(scene.buildIndex);

			if (networkSceneLoaded != null)
				networkSceneLoaded(scene, mode);

			BMSByte data = new BMSByte();
			ObjectMapper.Instance.MapBytes(data, scene.buildIndex, (int)mode);

			Binary frame = new Binary(Networker.Time.Timestep, false, data, Networker is IServer ? Receivers.All : Receivers.Server, MessageGroupIds.VIEW_CHANGE, Networker is BaseTCP);

			// Send the binary frame to either the server or the clients
			SendFrame(Networker, frame);

			// Go through all of the current NetworkBehaviors in the order that Unity finds them in
			// and associate them with the id that the network will be giving them as a lookup
			int currentAttachCode = 1;
			var behaviors = FindObjectsOfType<NetworkBehavior>().Where(b => !b.Initialized)
				.OrderBy(b => b.GetType().ToString())
				.OrderBy(b => b.name)
				.OrderBy(b => Vector3.Distance(Vector3.zero, b.transform.position))
				.ToList();

			if (behaviors.Count == 0)
			{
				if (Networker is IClient)
					NetworkObject.Flush(Networker);

				return;
			}

			foreach (NetworkBehavior behavior in behaviors)
			{
				behavior.TempAttachCode = scene.buildIndex << 16;
				behavior.TempAttachCode += currentAttachCode++;
				behavior.TempAttachCode = -behavior.TempAttachCode;
			}

			if (Networker is IClient)
			{
				NetworkObject foundNetworkObject;
				for (int i = 0; i < behaviors.Count; i++)
				{
					if (pendingNetworkObjects.TryGetValue(behaviors[i].TempAttachCode, out foundNetworkObject))
					{
						behaviors[i].Initialize(foundNetworkObject);
						behaviors.RemoveAt(i--);
					}
				}

				if (behaviors.Count == 0)
				{
					NetworkObject.Flush(Networker);
					return;
				}
			}

			if (Networker is IServer)
			{
				// Go through all of the pending NetworkBehavior objects and initialize them on the network
				foreach (INetworkBehavior behavior in behaviors)
					behavior.Initialize(Networker);

				return;
			}

			foreach (NetworkBehavior behavior in behaviors)
				pendingObjects.Add(behavior.TempAttachCode, behavior);

			//NetworkObject.objectCreated += CreatePendingObjects;
			NetworkObject.Flush(Networker);
		}
	}
}
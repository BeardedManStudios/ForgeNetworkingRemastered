﻿using System;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking.SQP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public partial class NetworkManager : MonoBehaviour
	{
		private const int UnloadSceneCommand = 2;

		public static NetworkManager Instance { get; private set; }

		public UnityAction<int, LoadSceneMode> networkSceneChanging;
		public UnityAction<Scene, LoadSceneMode> networkSceneLoaded;
		public event NetWorker.PlayerEvent playerLoadedScene;

		public NetWorker Networker { get; protected set; }
		public NetWorker MasterServerNetworker { get; protected set; }
		public Dictionary<int, INetworkBehavior> pendingObjects = new Dictionary<int, INetworkBehavior>();
		public Dictionary<int, NetworkObject> pendingNetworkObjects = new Dictionary<int, NetworkObject>();

		public ForgeSettings Settings;

		protected string _masterServerHost;
		protected ushort _masterServerPort;

		protected List<int> loadedScenes = new List<int>();
        protected List<int> loadingScenes = new List<int>();

		public bool IsServer { get { return Networker.IsServer; } }

		/// <summary>
		/// Used to enable or disable the automatic switching for clients
		/// </summary>
		public bool automaticScenes = true;

		/// <summary>
		/// Internal flag to indicate that the Initialize method has been called.
		/// </summary>
		protected bool initialized;

		/// <summary>
		/// The service that handles Server Query Protocol requests
		/// </summary>
		protected SQPServer sqpServer;

#if FN_WEBSERVER
		MVCWebServer.ForgeWebServer webserver = null;
#endif

		protected virtual void Awake()
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

			if (Settings == null)
			{
				Debug.LogError("No settings were provided. Trying to find default settings");
				Settings = FindObjectOfType<ForgeSettings>();
				if (Settings == null)
				{
					throw new BaseNetworkException("Could not find forge settings!");
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (automaticScenes)
			{
				SceneManager.sceneLoaded += SceneLoaded;
				SceneManager.sceneUnloaded += SceneUnloaded;
			}
		}

		protected virtual void OnDisable()
		{
			if (automaticScenes)
			{
				SceneManager.sceneLoaded -= SceneLoaded;
				SceneManager.sceneUnloaded -= SceneUnloaded;
			}
		}

		public virtual void Initialize(NetWorker networker, string masterServerHost = "", ushort masterServerPort = 15940, JSONNode masterServerRegisterData = null)
		{
			Networker = networker;
			networker.objectCreated += CreatePendingObjects;
			Networker.binaryMessageReceived += ReadBinary;
			SetupObjectCreatedEvent();

			UnityObjectMapper.Instance.UseAsDefault();
			NetworkObject.Factory = new NetworkObjectFactory();

			if (Networker is IServer)
			{
				if (Settings.enableSQP)
				{
					sqpServer = new SQPServer(Settings.SQPPort);
				}

				if (!string.IsNullOrEmpty(masterServerHost))
				{
					_masterServerHost = masterServerHost;
					_masterServerPort = masterServerPort;

					RegisterOnMasterServer(masterServerRegisterData);
				}

				Networker.playerAccepted += PlayerAcceptedSceneSetup;

#if FN_WEBSERVER
				string pathToFiles = "fnwww/html";
				Dictionary<string, string> pages = new Dictionary<string, string>();
				TextAsset[] assets = Resources.LoadAll<TextAsset>(pathToFiles);
				foreach (TextAsset a in assets)
					pages.Add(a.name, a.text);

				webserver = new MVCWebServer.ForgeWebServer(networker, pages);
				webserver.Start();
#endif
			}

			initialized = true;
		}

		protected virtual void CreatePendingObjects(NetworkObject obj)
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

			if (pendingObjects.Count == 0 && loadingScenes.Count == 0)
				Networker.objectCreated -= CreatePendingObjects;
		}

		public virtual void MatchmakingServersFromMasterServer(string masterServerHost,
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
			client.serverAccepted += (sender) =>
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
					client.Send(Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
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
			client.textMessageReceived += (player, frame, sender) =>
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

		public virtual JSONNode MasterServerRegisterData(NetWorker server, string id, string serverName, string type, string mode, string comment = "", bool useElo = false, int eloRequired = 0)
		{
			// Create the get request with the desired filters
			JSONNode sendData = JSONNode.Parse("{}");
			JSONClass registerData = new JSONClass();
			registerData.Add("id", id);
			registerData.Add("name", serverName);
			registerData.Add("port", new JSONData(server.Port));
			registerData.Add("playerCount", new JSONData(server.Players.Count));
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

		protected virtual void RegisterOnMasterServer(JSONNode masterServerData)
		{
			// The Master Server communicates over TCP
			TCPMasterClient client = new TCPMasterClient();

			// Once this client has been accepted by the master server it should send it's get request
			client.serverAccepted += (sender) =>
			{
				try
				{
					Text temp = Text.CreateFromString(client.Time.Timestep, masterServerData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_REGISTER, true);

					//Debug.Log(temp.GetData().Length);
					// Send the request to the server
					client.Send(temp);

					Networker.disconnected += s =>
					{
						client.Disconnect(false);
						MasterServerNetworker = null;
					};
				}
				catch
				{
					// If anything fails, then this client needs to be disconnected
					client.Disconnect(true);
					client = null;
				}
			};

			client.Connect(_masterServerHost, _masterServerPort);

			Networker.disconnected += NetworkerDisconnected;
			MasterServerNetworker = client;
		}

		protected virtual void NetworkerDisconnected(NetWorker sender)
		{
			Networker.disconnected -= NetworkerDisconnected;
			MasterServerNetworker.Disconnect(false);
			MasterServerNetworker = null;
		}

		public virtual void UpdateMasterServerListing(NetWorker server, string comment = null, string gameType = null, string mode = null)
		{
			JSONNode sendData = JSONNode.Parse("{}");
			JSONClass registerData = new JSONClass();

			registerData.Add("playerCount", new JSONData(server.Players.Count));
			if (comment != null) registerData.Add("comment", comment);
			if (gameType != null) registerData.Add("type", gameType);
			if (mode != null) registerData.Add("mode", mode);
			registerData.Add("port", new JSONData(server.Port));

			sendData.Add("update", registerData);

			UpdateMasterServerListing(sendData);
		}

		protected virtual void UpdateMasterServerListing(JSONNode masterServerData)
		{
			if (string.IsNullOrEmpty(_masterServerHost))
			{
				throw new System.Exception("This server is not registered on a master server, please ensure that you are passing a master server host and port into the initialize");
			}

			if (MasterServerNetworker == null)
			{
				throw new System.Exception("Connection to master server is closed. Make sure to be connected to master server before update trial");
			}

			// The Master Server communicates over TCP
			TCPMasterClient client = new TCPMasterClient();

			// Once this client has been accepted by the master server it should send it's update request
			client.serverAccepted += (sender) =>
			{
				try
				{
					Text temp = Text.CreateFromString(client.Time.Timestep, masterServerData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_UPDATE, true);

					// Send the request to the server
					client.Send(temp);
				}
				finally
				{
					// If anything fails, then this client needs to be disconnected
					client.Disconnect(true);
					client = null;
				}
			};

			client.Connect(_masterServerHost, _masterServerPort);
		}

		public virtual void Disconnect()
		{
#if FN_WEBSERVER
			webserver.Stop();
#endif

			Networker.objectCreated -= CreatePendingObjects;

			if (Networker != null)
				Networker.Disconnect(false);

			if (sqpServer != null)
				sqpServer.ShutDown();

			NetWorker.EndSession();

			NetworkObject.ClearNetworkObjects(Networker);
			pendingObjects.Clear();
			pendingNetworkObjects.Clear();
			MasterServerNetworker = null;
			Networker = null;
			Instance = null;
			Destroy(gameObject);
		}

		protected virtual void OnApplicationQuit()
		{
			if (Networker != null)
				Networker.Disconnect(false);

			if (sqpServer != null)
				sqpServer.ShutDown();

			NetWorker.EndSession();
		}

		protected virtual void Update()
		{
			if (Networker != null)
			{
				for (int i = 0; i < Networker.NetworkObjectList.Count; i++)
					Networker.NetworkObjectList[i].InterpolateUpdate();
			}

			if (sqpServer != null)
			{
				UpdateSQPServer();
			}
		}

		protected virtual void UpdateSQPServer()
		{
			// Update SQP data with current values
			var sid = sqpServer.ServerInfoData;

			sid.Port = Networker.Port;
			// This count will include the host, for dedicated server setups it needs to be count-1
			sid.CurrentPlayers = Convert.ToUInt16(Networker.Players.Count);
			sid.MaxPlayers = Convert.ToUInt16(Networker.MaxConnections);
			sid.ServerName = Settings.serverName;
			sid.ServerType = Settings.type;

			sqpServer.Update();
		}

		protected virtual void ProcessOthers(Transform obj, NetworkObject createTarget, ref uint idOffset, NetworkBehavior netBehavior = null)
		{
			int i;

			// Get the order of the components as they are in the inspector
			var components = obj.GetComponents<NetworkBehavior>();

			// Create each network object that is available
			for (i = 0; i < components.Length; i++)
			{
				if (components[i] == netBehavior)
					continue;

				var no = components[i].CreateNetworkObject(Networker, 0);

				if (Networker.IsServer)
					FinalizeInitialization(obj.gameObject, components[i], no, obj.position, obj.rotation, false, true);
				else
					components[i].AwaitNetworkBind(Networker, createTarget, idOffset++);
			}

			for (i = 0; i < obj.transform.childCount; i++)
				ProcessOthers(obj.transform.GetChild(i), createTarget, ref idOffset);
		}

		protected virtual void FinalizeInitialization(GameObject go, INetworkBehavior netBehavior, NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true, bool skipOthers = false)
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

			if (!skipOthers)
			{
				// Go through all associated network behaviors in the hierarchy (including self) and
				// Assign their TempAttachCode for lookup later. Should use an incrementor or something
				uint idOffset = 1;
				ProcessOthers(go.transform, obj, ref idOffset, (NetworkBehavior)netBehavior);
			}

			go.SetActive(true);
		}

		/// <summary>
		/// Called automatically when a new player is accepted and sends the player
		/// the currently loaded scene indexes for the client to load
		/// </summary>
		/// <param name="player">The player that was just accepted</param>
		/// <param name="sender">The sending <see cref="NetWorker"/></param>
		protected virtual void PlayerAcceptedSceneSetup(NetworkingPlayer player, NetWorker sender)
		{
			BMSByte data = ObjectMapper.BMSByte(loadedScenes.Count);

			// Go through all the loaded scene indexes and send them to the connecting player
			for (int i = 0; i < loadedScenes.Count; i++)
				ObjectMapper.Instance.MapBytes(data, loadedScenes[i]);

			Binary frame = new Binary(sender.Time.Timestep, false, data, Receivers.Target, MessageGroupIds.VIEW_INITIALIZE, sender is BaseTCP);

			SendFrame(sender, frame, player);
		}

		protected virtual void ReadBinary(NetworkingPlayer player, Binary frame, NetWorker sender)
		{
			if (frame.GroupId == MessageGroupIds.VIEW_INITIALIZE)
			{
				if (Networker is IServer)
					return;

				int count = frame.StreamData.GetBasicType<int>();

                loadingScenes.Clear();
				for (int i = 0; i < count; i++)
                    loadingScenes.Add(frame.StreamData.GetBasicType<int>());

                int[] scenesToLoad = loadingScenes.ToArray();
				MainThreadManager.Run(() =>
				{
					if (scenesToLoad.Length == 0)
						return;

					SceneManager.LoadScene(scenesToLoad[0], LoadSceneMode.Single);

					for (int i = 1; i < scenesToLoad.Length; i++)
						SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
				});

				return;
			}

			if (frame.GroupId != MessageGroupIds.VIEW_CHANGE)
				return;

			if (Networker.IsServer)
			{
				// The client has loaded the scene
				if (playerLoadedScene != null)
					playerLoadedScene(player, Networker);

				return;
			}

			// Get the sceme index we are talking about
			int sceneIndex = frame.StreamData.GetBasicType<int>();

			// Get the mode in which the server loaded the scene
			int modeIndex = frame.StreamData.GetBasicType<int>();

			if (modeIndex == UnloadSceneCommand)
			{
				MainThreadManager.Run(() =>
				{
					UnloadSceneAdditive(sceneIndex);
				});
			}
			else
			{
				LoadSceneMode mode;
				lock (NetworkObject.PendingCreatesLock)
				{
					// We need to halt the creation of network objects until we load the scene
					Networker.PendCreates = true;

					// Convert the int mode to the enum mode
					mode = (LoadSceneMode)modeIndex;

					if (mode == LoadSceneMode.Single)
						loadingScenes.Clear();

					loadingScenes.Add(sceneIndex);
				}

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
#if STEAMWORKS
					else if (networker is SteamP2PServer)
						((SteamP2PServer)networker).Send(targetPlayer, frame, true);
#endif
					else
						((UDPServer)networker).Send(targetPlayer, frame, true);
				}
				else
				{
					if (networker is TCPServer)
						((TCPServer)networker).SendAll(frame);
#if STEAMWORKS
					else if (networker is SteamP2PServer)
						((SteamP2PServer)networker).Send(frame, true);
#endif
					else
						((UDPServer)networker).Send(frame, true);
				}
			}
			else
			{
				if (networker is TCPClientBase)
					((TCPClientBase)networker).Send(frame);
#if STEAMWORKS
				else if (networker is SteamP2PClient)
					((SteamP2PClient)networker).Send(frame, true);
#endif
				else
					((UDPClient)networker).Send(frame, true);
			}
		}

		public virtual void SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			// The NetworkManager has not yet been initialized with a Networker.
			if (!initialized)
				return;

			// If we are loading a completely new scene then we will need
			// to clear out all the old objects that were stored as they
			// are no longer needed
			if (mode != LoadSceneMode.Additive)
			{
				pendingObjects.Clear();
				pendingNetworkObjects.Clear();
				loadedScenes.Clear();
			}
            lock(NetworkObject.PendingCreatesLock)
            {
                loadingScenes.Remove(scene.buildIndex);
            }
			loadedScenes.Add(scene.buildIndex);


			if (networkSceneLoaded != null)
				networkSceneLoaded(scene, mode);

			BMSByte data = ObjectMapper.BMSByte(scene.buildIndex, (int)mode);

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
                {
                    if (loadingScenes.Count > 0)
                        NetworkObject.Flush(Networker, loadingScenes, CreatePendingObjects);
                    else
                    {
                        NetworkObject.Flush(Networker, loadingScenes);
                        if(pendingObjects.Count == 0)
                            Networker.objectCreated -= CreatePendingObjects;
                    }
                }


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
                // This would occur if objects in the additive scene arrives at the same time as the
                // "single" scene and were flushed.
                if (mode == LoadSceneMode.Additive && pendingNetworkObjects.Count > 0)
                {
                    NetworkObject foundNetworkObject;
                    for (int i = 0; i < behaviors.Count; i++)
                    {
                        if (pendingNetworkObjects.TryGetValue(behaviors[i].TempAttachCode, out foundNetworkObject))
                        {
                            behaviors[i].Initialize(foundNetworkObject);
                            pendingNetworkObjects.Remove(behaviors[i].TempAttachCode);
                            behaviors.RemoveAt(i--);
                        }
                    }
                }

                foreach (NetworkBehavior behavior in behaviors)
                    pendingObjects.Add(behavior.TempAttachCode, behavior);

                NetworkObject.Flush(Networker, loadingScenes, CreatePendingObjects);

                if (pendingObjects.Count == 0 && loadingScenes.Count == 0)
                    Networker.objectCreated -= CreatePendingObjects;
				else if (pendingObjects.Count != 0 && loadingScenes.Count == 0)
                {
	                // Pending network behavior list is not empty when there are no more scenes to load.
	                // Probably network behaviours that were placed in the scene have already been destroyed on the server and other clients!

	                List<GameObject> objetsToDestroy = new List<GameObject>(pendingObjects.Count);
	                foreach (var behavior in pendingObjects.Values)
	                {
		                var gameObject = ((NetworkBehavior) behavior).gameObject;
		                if (!objetsToDestroy.Contains(gameObject))
			                objetsToDestroy.Add(gameObject);
	                }

	                pendingObjects.Clear();

	                foreach (var o in objetsToDestroy)
	                {
		                Destroy(o);
	                }

	                objetsToDestroy.Clear();
                }

            } else
			{
				// Go through all of the pending NetworkBehavior objects and initialize them on the network
				foreach (INetworkBehavior behavior in behaviors)
					behavior.Initialize(Networker);

			}
		}

		/// <summary>
		/// A helper function to retrieve a GameObject by its network id.
		/// </summary>
		/// <param name="id">Network id of the gameobject</param>
        public GameObject GetGameObjectByNetworkId(uint id)
        {
            if (Networker == null ) //Only check Networker, as NetworkObjects are always initiliased.
            {
                Debug.LogWarning("Networker is null. Network manager has not been initiliased.");
                return null;
            }

            NetworkObject foundNetworkObject = null;
			if (!Networker.NetworkObjects.TryGetValue(id, out foundNetworkObject) || foundNetworkObject.AttachedBehavior == null)
            {
                Debug.LogWarning("No object found by id or object has no attached behavior.");
                return null;
            }

            return ((NetworkBehavior)foundNetworkObject.AttachedBehavior).gameObject;
        }

		/// <summary>
		/// Callback for when a Scene has been unloaded
		/// </summary>
		/// <param name="scene"></param>
		public virtual void SceneUnloaded(Scene scene)
		{
			// The NetworkManager has not yet been initialized with a Networker.
			if (!initialized)
				return;

			loadedScenes.Remove(scene.buildIndex);

			// Send buildindex and 2 refering to
			BMSByte data = ObjectMapper.BMSByte(scene.buildIndex, UnloadSceneCommand);

			Binary frame = new Binary(Networker.Time.Timestep, false, data, Networker is IServer ? Receivers.All : Receivers.Server, MessageGroupIds.VIEW_CHANGE, false);

			// Send the binary frame to either the server or the clients
			SendFrame(Networker, frame);
		}

		/// <summary>
		/// Unloads a scene from the network
		/// You should only use this if the scene you want to unload is a additive scene
		/// </summary>
		/// <param name="buildIndex"></param>
		public void UnloadSceneAdditive(int buildIndex)
		{
			if (buildIndex < 0)
				return;

			Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
			UnloadSceneAdditive(scene);
		}

		/// <summary>
		/// Unloads a scene from the network
		/// You should only use this if the scene you want to unload is a additive scene
		/// </summary>
		/// <param name="sceneName"></param>
		public void UnloadSceneAdditive(string sceneName)
		{
			Scene scene = SceneManager.GetSceneByName(sceneName);
			UnloadSceneAdditive(scene);
		}

		/// <summary>
		/// Unloads a networked scene which must be additive
		/// </summary>
		/// <param name="scene"></param>
		void UnloadSceneAdditive(Scene scene)
		{
			if (scene.IsValid())
			{
				UnloadSceneNetworkObjects(scene.buildIndex);
				SceneManager.UnloadSceneAsync(scene.buildIndex);
			}
		}

		/// <summary>
		/// Called when you want to remove all network objects from the Networker list for a scene
		/// </summary>
		/// <param name="buildIndex"></param>
		void UnloadSceneNetworkObjects(int buildIndex)
		{
			if (buildIndex >= 0)
			{
				List<NetworkObject> networkObjectsToDestroy = new List<NetworkObject>();

				// Gets all networkObjects related to the scene we are destorying
				Networker.IterateNetworkObjects(networkObject =>
				{
					NetworkBehavior networkBehavior = (NetworkBehavior)networkObject.AttachedBehavior;
					if (networkBehavior && networkBehavior.gameObject)
					{
						if (networkBehavior.gameObject.scene.buildIndex == buildIndex)
						{
							networkObjectsToDestroy.Add(networkObject);
						}
					}
				});

				Networker.ManualRemove(networkObjectsToDestroy);

				foreach (NetworkObject networkObject in networkObjectsToDestroy)
				{
					pendingNetworkObjects.Remove(networkObject.CreateCode);
				}
			}
		}
	}
}

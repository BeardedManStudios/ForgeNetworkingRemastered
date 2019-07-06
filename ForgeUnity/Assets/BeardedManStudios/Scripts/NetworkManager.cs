using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Exceptions;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.MasterServer;
using BeardedManStudios.Forge.Networking.Unity.RemoteWorkers;
using BeardedManStudios.SimpleJSON;
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

		public NetWorker Networker { get; protected set; }
		public NetWorker MasterServerNetworker { get; protected set; }
		public Dictionary<int, INetworkBehavior> pendingObjects = new Dictionary<int, INetworkBehavior>();
		public Dictionary<int, NetworkObject> pendingNetworkObjects = new Dictionary<int, NetworkObject>();
		protected string masterServerHost;
		protected ushort masterServerPort;

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
			DontDestroyOnLoad(gameObject);
		}

		protected virtual void OnEnable()
		{
			if (automaticScenes)
				SceneManager.sceneLoaded += SceneReady;
		}

		protected virtual void OnDisable()
		{
			if (automaticScenes)
				SceneManager.sceneLoaded -= SceneReady;
		}

		public virtual void Initialize(NetWorker networker, string masterServerHost = "", ushort masterServerPort = 15940, JSONNode masterServerRegisterData = null)
		{
			PrepareNetworkerInstance(networker);
			UnityObjectMapper.Instance.UseAsDefault();
			NetworkObject.Factory = new NetworkObjectFactory();
			InitializeServerElements(masterServerHost, masterServerPort, masterServerRegisterData);
			initialized = true;
		}

		private void InitializeServerElements(string masterServerHost, ushort masterServerPort, JSONNode masterServerRegisterData)
		{
			if (Networker is IServer)
			{
				Networker.playerAccepted += PlayerAcceptedSceneSetup;
				SetupMasterOnMasterServer(masterServerHost, masterServerPort, masterServerRegisterData);
#if FN_WEBSERVER
				InitializeWebServer(networker);
#endif
			}
		}

		private void PrepareNetworkerInstance(NetWorker networker)
		{
			Networker = networker;
			networker.objectCreated += CreatePendingObjects;
			Networker.binaryMessageReceived += ReadBinary;
			SetupObjectCreatedEvent();
		}

		private void SetupMasterOnMasterServer(string masterServerHost, ushort masterServerPort, JSONNode masterServerRegisterData)
		{
			if (!string.IsNullOrEmpty(masterServerHost))
			{
				this.masterServerHost = masterServerHost;
				this.masterServerPort = masterServerPort;

				RegisterOnMasterServer(masterServerRegisterData);
			}
		}

#if FN_WEBSERVER
		private static void InitializeWebServer(NetWorker networker)
		{
			string pathToFiles = "fnwww/html";
			var pages = new Dictionary<string, string>();
			TextAsset[] assets = Resources.LoadAll<TextAsset>(pathToFiles);
			foreach (TextAsset a in assets)
				pages.Add(a.name, a.text);
			webserver = new MVCWebServer.ForgeWebServer(networker, pages);
			webserver.Start();
		}
#endif

		protected virtual void CreatePendingObjects(NetworkObject obj)
		{
			INetworkBehavior behavior;
			if (!pendingObjects.TryGetValue(obj.CreateCode, out behavior))
			{
				AddNetworkObjectToPendingList(obj);
				return;
			}
			MakeFoundPendingNetworkObjectActive(obj, behavior);
			CleanUpPendingObjectsIfNeeded();
		}

		private void AddNetworkObjectToPendingList(NetworkObject obj)
		{
			if (obj.CreateCode < 0)
				pendingNetworkObjects.Add(obj.CreateCode, obj);
			else
				BMSLogger.Instance.Log("A network object with id " + obj.NetworkId + " was being added to pending without a create code");
		}

		private void MakeFoundPendingNetworkObjectActive(NetworkObject obj, INetworkBehavior behavior)
		{
			behavior.Initialize(obj);
			pendingObjects.Remove(obj.CreateCode);
		}

		private void CleanUpPendingObjectsIfNeeded()
		{
			if (pendingObjects.Count == 0 && loadingScenes.Count == 0)
				Networker.objectCreated -= CreatePendingObjects;
		}

		public virtual void MatchmakingServersFromMasterServer(string masterServerHost, ushort masterServerPort, int elo,
			System.Action<MasterServerResponse> callback = null, string gameId = "myGame", string gameType = "any",
			string gameMode = "all")
		{
			var fetcher = MasterServerRequestManger.CreateWithData(new MasterServerRequestManger.RequestData
			{
				completedCallback = callback,
				elo = elo,
				gameId = gameId,
				gameMode = gameMode,
				gameType = gameType,
				hostAddress = masterServerHost,
				hostPort = masterServerPort
			});
			fetcher.SendRegisterRequest();
		}

		public virtual JSONNode MasterServerRegisterData(NetWorker server, string id, string serverName, string type,
			string mode, string comment = "", bool useElo = false, int eloRequired = 0)
		{
			return TCPMasterClient.CreateMasterServerRegisterData(server, id, serverName, type,
				mode, comment, useElo, eloRequired);
		}

		protected virtual void RegisterOnMasterServer(JSONNode masterServerData)
		{
			// The Master Server communicates over TCP
			var client = new TCPMasterClient();
			client.RegisterOnMasterServer(masterServerHost, masterServerPort, masterServerData);
			MasterServerNetworker = client;
			Networker.disconnected += DisconnectFromMasterServer;
		}

		protected virtual void DisconnectFromMasterServer(NetWorker sender)
		{
			Networker.disconnected -= DisconnectFromMasterServer;
			MasterServerNetworker.Disconnect(false);
			MasterServerNetworker = null;
		}

		public virtual void UpdateMasterServerListing(NetWorker server, string comment = null,
			string gameType = null, string gameMode = null)
		{
			if (MasterServerNetworker == null)
				throw new NotConnectedToMasterServerException("Connection to master server is closed. Make sure to be connected to master server before updating the server listing");

			var fetcher = MasterServerRequestManger.CreateWithData(new MasterServerRequestManger.RequestData
			{
				gameMode = gameMode,
				gameType = gameType,
				hostAddress = masterServerHost,
				hostPort = masterServerPort,
				hostedServerPort = server.Port,
				comment = comment,
				playerCount = server.Players.Count
			});
			fetcher.SendUpdateRequest();
		}

		public virtual void Disconnect()
		{
#if FN_WEBSERVER
			webserver.Stop();
#endif

			Networker.objectCreated -= CreatePendingObjects;

			if (Networker != null)
				Networker.Disconnect(false);

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
			NetWorker.EndSession();
		}

		protected virtual void Update()
		{
			if (Networker != null)
			{
				for (int i = 0; i < Networker.NetworkObjectList.Count; i++)
					Networker.NetworkObjectList[i].InterpolateUpdate();
			}
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
			if (Networker.IsServer)
				InitializedObject(netBehavior, obj);
			else
				obj.pendingInitialized += InitializedObject;

			SetInitializationTransformations(go, position, rotation);
			if (!skipOthers)
			{
				// Go through all associated network behaviors in the hierarchy (including self) and
				// Assign their TempAttachCode for lookup later. Should use an incrementor or something
				uint idOffset = 1;
				ProcessOthers(go.transform, obj, ref idOffset, (NetworkBehavior)netBehavior);
			}
		}

		private static void SetInitializationTransformations(GameObject go, Vector3? position, Quaternion? rotation)
		{
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
		}

		/// <summary>
		/// Called automatically when a new player is accepted and sends the player
		/// the currently loaded scene indexes for the client to load
		/// </summary>
		/// <param name="player">The player that was just accepted</param>
		/// <param name="sender">The sending <see cref="NetWorker"/></param>
		protected virtual void PlayerAcceptedSceneSetup(NetworkingPlayer player, NetWorker sender)
		{
			var data = PrepareAllLoadedSceneIndexesForConnnectingPlayer();
			Binary frame = new Binary(sender.Time.Timestep, false, data, Receivers.Target, MessageGroupIds.VIEW_INITIALIZE, sender is BaseTCP);
			SendFrame(sender, frame, player);
		}

		private BMSByte PrepareAllLoadedSceneIndexesForConnnectingPlayer()
		{
			BMSByte data = ObjectMapper.BMSByte(loadedScenes.Count);
			for (int i = 0; i < loadedScenes.Count; i++)
				ObjectMapper.Instance.MapBytes(data, loadedScenes[i]);
			return data;
		}

		protected virtual void ReadBinary(NetworkingPlayer player, Binary frame, NetWorker sender)
		{
			if (frame.GroupId == MessageGroupIds.VIEW_INITIALIZE)
			{
				if (!Networker.IsServer)
					ExecuteSceneViewInitializeOnClient(frame);
				return;
			}

			if (frame.GroupId == MessageGroupIds.VIEW_CHANGE)
				ProcessViewChange(player, frame);
		}

		private void ExecuteSceneViewInitializeOnClient(Binary frame)
		{
			int count = frame.StreamData.GetBasicType<int>();
			loadingScenes.Clear();
			for (int i = 0; i < count; i++)
				loadingScenes.Add(frame.StreamData.GetBasicType<int>());
			var allSceneLoader = new RemoteSceneLoader();
			allSceneLoader.AssignAllIndexes(loadingScenes);
			MainThreadManager.Run(allSceneLoader.LoadAllScenes);
		}

		private void ProcessViewChange(NetworkingPlayer player, Binary frame)
		{
			if (Networker.IsServer)
			{
				// The client has loaded the scene
				if (playerLoadedScene != null)
					playerLoadedScene(player, Networker);
			}
			else
				ClientExecuteRemoteLoadScene(frame);
		}

		private void ClientExecuteRemoteLoadScene(Binary frame)
		{
			RemoteSceneLoader loader;
			lock (NetworkObject.PendingCreatesLock)
			{
				loader = SetupRemoteSceneLoader(frame);
			}

			if (networkSceneChanging != null)
				networkSceneChanging(loader.FirstSceneIndex, loader.SceneMode);

			MainThreadManager.Run(loader.ExecuteRemoteSceneLoad);
		}

		private RemoteSceneLoader SetupRemoteSceneLoader(Binary frame)
		{
			// We need to halt the creation of network objects until we load the scene
			Networker.PendCreates = true;
			var loader = new RemoteSceneLoader();
			int sceneIndex = frame.StreamData.GetBasicType<int>();
			var mode = (LoadSceneMode)frame.StreamData.GetBasicType<int>();
			if (mode == LoadSceneMode.Single)
				loadingScenes.Clear();
			loadingScenes.Add(sceneIndex);
			loader.AssignFirst(sceneIndex, mode);
			return loader;
		}

		private static void LoadSceneFromServer(int sceneIndex, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Additive)
				SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
			else if (mode == LoadSceneMode.Single)
				SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
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
				var server = (IServer)networker;
				if (targetPlayer != null)
					server.SendReliableToPlayer(targetPlayer, frame);
				else
					server.SendReliable(frame);
			}
			else
			{
				var client = (IClient)networker;
				client.SendReliable(frame);
			}
		}

		public virtual void SceneReady(Scene scene, LoadSceneMode mode)
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
			lock (NetworkObject.PendingCreatesLock)
			{
				loadingScenes.Remove(scene.buildIndex);
			}
			loadedScenes.Add(scene.buildIndex);

			if (networkSceneLoaded != null)
				networkSceneLoaded(scene, mode);

			BMSByte data = ObjectMapper.BMSByte(scene.buildIndex, (int)mode);
			var frame = new Binary(Networker.Time.Timestep, false, data, Networker is IServer ? Receivers.All : Receivers.Server, MessageGroupIds.VIEW_CHANGE, Networker is BaseTCP);

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
						if (pendingObjects.Count == 0)
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
					// Probably network behaviours that were placed in the scene have already been
					// destroyed on the server and other clients!
					DestroyAllGameObjectsInPendingObjectList();
					pendingObjects.Clear();
				}
			}
			else
				InitializeNetworkBehaviorsInList(behaviors);
		}

		private void DestroyAllGameObjectsInPendingObjectList()
		{
			var objetsToDestroy = GetUniqueGameObjectListFromPendingNetworkObjectList();
			foreach (var o in objetsToDestroy)
				Destroy(o);
			objetsToDestroy.Clear();
		}

		private List<GameObject> GetUniqueGameObjectListFromPendingNetworkObjectList()
		{
			var gameObjects = new List<GameObject>(pendingObjects.Count);
			foreach (var behavior in pendingObjects.Values)
			{
				var gameObject = ((NetworkBehavior)behavior).gameObject;
				if (!gameObjects.Contains(gameObject))
					gameObjects.Add(gameObject);
			}
			return gameObjects;
		}

		private void InitializeNetworkBehaviorsInList(List<NetworkBehavior> behaviors)
		{
			foreach (INetworkBehavior behavior in behaviors)
				behavior.Initialize(Networker);
		}

		/// <summary>
		/// A helper function to retrieve a GameObject by its network id.
		/// </summary>
		/// <param name="id">Network id of the gameobject</param>
		public GameObject GetGameObjectByNetworkId(uint id)
		{
			var foundNetworkObject = TryToLocateNetworkObjectInNetworker(id);
			if (foundNetworkObject != null)
				return ((NetworkBehavior)foundNetworkObject.AttachedBehavior).gameObject;
			return null;
		}

		private NetworkObject TryToLocateNetworkObjectInNetworker(uint id)
		{
			NetworkObject foundNetworkObject = null;
			if (Networker == null)
				BMSLogger.Instance.LogWarning("Networker is null, it has not been initialized yet");
			else if (!Networker.NetworkObjects.TryGetValue(id, out foundNetworkObject) || foundNetworkObject.AttachedBehavior == null)
				BMSLogger.Instance.LogWarning("No object found by id or object has no attached behavior.");
			return foundNetworkObject;
		}
	}
}

using BeardedManStudios.Forge.Networking.Generated;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public partial class NetworkManager : MonoBehaviour
	{
		public static NetworkManager Instance { get; private set; }

		public NetWorker Networker { get; private set; }
		public NetWorker MasterServerNetworker { get; private set; }
		public Dictionary<int, INetworkBehavior> pendingObjects = new Dictionary<int, INetworkBehavior>();
		public Dictionary<int, NetworkObject> pendingNetworkObjects = new Dictionary<int, NetworkObject>();

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
			//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
		}

		private void OnDisable()
		{
			//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
			SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		}

		public void Initialize(NetWorker networker, string masterServerHost = "", ushort masterServerPort = 15940, bool useElo = false, int eloRequired = 0)
		{
			Networker = networker;

			UnityObjectMapper.Instance.UseAsDefault();
			NetworkObject.Factory = new NetworkObjectFactory();

			if (Networker is IServer)
			{
				if (!string.IsNullOrEmpty(masterServerHost))
					RegisterOnMasterServer(15937, 32, masterServerHost, masterServerPort);
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

		private void RegisterOnMasterServer(ushort port, int maxPlayers, string masterServerHost, ushort masterServerPort, bool useElo = false, int eloRequired = 0)
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
					JSONClass registerData = new JSONClass();
					registerData.Add("id", "myGame");
					registerData.Add("name", "Forge Game");
					registerData.Add("port", new JSONData(port));
					registerData.Add("playerCount", new JSONData(0));
					registerData.Add("maxPlayers", new JSONData(maxPlayers));
					registerData.Add("comment", "Demo comment...");
					registerData.Add("type", "Deathmatch");
					registerData.Add("mode", "Teams");
					registerData.Add("protocol", "udp");
					registerData.Add("elo", new JSONData(eloRequired));
					registerData.Add("useElo", new JSONData(useElo));

					sendData.Add("register", registerData);

					Frame.Text temp = Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_REGISTER, true);

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

		private void OnApplicationQuit()
		{
			if (Networker != null)
				Networker.Disconnect(false);

			NetWorker.ApplicationExit();
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
					if (sendTransform)
						obj.SendRpc("SetupTransform", Receivers.OthersBuffered, position.Value, rotation.Value);

					go.transform.position = position.Value;
					go.transform.rotation = rotation.Value;
				}
				else
				{
					if (sendTransform)
						obj.SendRpc("SetupPosition", Receivers.OthersBuffered, position.Value);

					go.transform.position = position.Value;
				}
			}

			if (!skipOthers)
			{
				// Go through all associated network behaviors in the hierarchy (including self) and
				// Assign their TempAttachCode for lookup later. Should use an incrementor or something
				ProcessOthers(go.transform, obj, 1, (NetworkBehavior)netBehavior);
			}
		}

		public void SceneReady(Scene scene, LoadSceneMode mode)
		{
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
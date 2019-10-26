using System;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BeardedManStudios.MultiplayerMenu
{
	public class MultiplayerMenu : MonoBehaviour
	{
		public InputField ipAddress = null;
		public InputField portNumber = null;
		public ForgeSettings Settings;

		public GameObject networkManager = null;
		public GameObject[] ToggledButtons;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool DontChangeSceneOnConnect = false;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public string masterServerHost = string.Empty;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public ushort masterServerPort = 15940;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public string natServerHost = string.Empty;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public ushort natServerPort = 15941;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool connectUsingMatchmaking = false;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool useElo = false;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public int myElo = 0;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public int eloRequired = 0;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool useMainThreadManagerForRPCs = true;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool useInlineChat = false;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool getLocalNetworkConnections = false;

		[Obsolete("This field is deprecated. Please use the new ForgeSettings ScriptableObject. The default settings can be found in 'Scripts/Default Forge Settings'")]
		public bool useTCP = false;


		private NetworkManager mgr = null;
		private NetWorker server;

		private List<Button> _uiButtons = new List<Button>();
		private bool _matchmaking = false;

		private JoinMenu JoinMenu;

		private void Awake()
		{
			if (Settings == null)
			{
				Debug.LogError("No settings were provided. Trying to find default settings.");
				Debug.LogWarning("Please check the settings in the inspector. If you are using non default values then you can use the update button on the menu component in the inspector to save them.");
				Settings = FindObjectOfType<ForgeSettings>();
				if (Settings == null)
				{
					throw new BaseNetworkException("Could not find forge settings! Please make sure you update your Multiplayer Menu settings in the editor");
				}
			}
		}

		private void Start()
		{
			ipAddress.text = "127.0.0.1";
			portNumber.text = "15937";

			JoinMenu = GetComponent<JoinMenu>();

			for (int i = 0; i < ToggledButtons.Length; ++i)
			{
				Button btn = ToggledButtons[i].GetComponent<Button>();
				if (btn != null)
					_uiButtons.Add(btn);
			}

			if (!Settings.useTCP)
			{
				// Do any firewall opening requests on the operating system
				NetWorker.PingForFirewall(ushort.Parse(portNumber.text));
			}

			if (Settings.useMainThreadManagerForRPCs)
				Rpc.MainThreadRunner = MainThreadManager.Instance;

			if (Settings.getLocalNetworkConnections)
			{
				NetWorker.localServerLocated += LocalServerLocated;
				NetWorker.RefreshLocalUdpListings(ushort.Parse(portNumber.text));
			}
		}

		private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
		{
			//Debug.Log("Found endpoint: " + endpoint.Address + ":" + endpoint.Port);
		}

		public void Connect()
		{
			if (Settings.connectUsingMatchmaking)
			{
				ConnectToMatchmaking();
				return;
			}
			ushort port;
			if(!ushort.TryParse(portNumber.text, out port))
			{
				Debug.LogError("The supplied port number is not within the allowed range 0-" + ushort.MaxValue);
				return;
			}

			NetWorker client;

			if (Settings.useTCP)
			{
				client = new TCPClient();
				((TCPClient)client).Connect(ipAddress.text, (ushort)port);
			}
			else
			{
				client = new UDPClient();
				if (Settings.natServerHost.Trim().Length == 0)
					((UDPClient)client).Connect(ipAddress.text, (ushort)port);
				else
					((UDPClient)client).Connect(ipAddress.text, (ushort)port, Settings.natServerHost, Settings.natServerPort);
			}

			Connected(client);
		}

		public void ConnectToMatchmaking()
		{
			if (_matchmaking)
				return;

			SetToggledButtons(false);
			_matchmaking = true;

			if (mgr == null && networkManager == null)
				throw new System.Exception("A network manager was not provided, this is required for the tons of fancy stuff");

			mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

			mgr.MatchmakingServersFromMasterServer(Settings.masterServerHost, Settings.masterServerPort, Settings.myElo, (response) =>
			{
				_matchmaking = false;
				SetToggledButtons(true);
				Debug.LogFormat("Matching Server(s) count[{0}]", response.serverResponse.Count);

				//TODO: YOUR OWN MATCHMAKING EXTRA LOGIC HERE!
				// I just make it randomly pick a server... you can do whatever you please!
				if (response != null && response.serverResponse.Count > 0)
				{
					MasterServerResponse.Server server = response.serverResponse[Random.Range(0, response.serverResponse.Count)];
					//TCPClient client = new TCPClient();
					UDPClient client = new UDPClient();
					client.Connect(server.Address, server.Port);
					Connected(client);
				}
			});
		}

		public void Host()
		{
			if (Settings.useTCP)
			{
				server = new TCPServer(64);
				((TCPServer)server).Connect();
			}
			else
			{
				server = new UDPServer(64);

				if (Settings.natServerHost.Trim().Length == 0)
					((UDPServer)server).Connect(ipAddress.text, ushort.Parse(portNumber.text));
				else
					((UDPServer)server).Connect(port: ushort.Parse(portNumber.text), natHost: Settings.natServerHost, natPort: Settings.natServerPort);
			}

			server.playerTimeout += (player, sender) =>
			{
				Debug.Log("Player " + player.NetworkId + " timed out");
			};
			//LobbyService.Instance.Initialize(server);

			Connected(server);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.H))
				Host();
			else if (Input.GetKeyDown(KeyCode.C))
				Connect();
			else if (Input.GetKeyDown(KeyCode.L))
			{
				NetWorker.localServerLocated -= TestLocalServerFind;
				NetWorker.localServerLocated += TestLocalServerFind;
				NetWorker.RefreshLocalUdpListings();
			}
		}

		private void TestLocalServerFind(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
		{
			Debug.Log("Address: " + endpoint.Address + ", Port: " + endpoint.Port + ", Server? " + endpoint.IsServer);
		}

		public void Connected(NetWorker networker)
		{
			if (!networker.IsBound)
			{
				Debug.LogError("NetWorker failed to bind");
				return;
			}

			if (mgr == null && networkManager == null)
			{
				Debug.LogWarning("A network manager was not provided, generating a new one instead");
				networkManager = new GameObject("Network Manager");
				mgr = networkManager.AddComponent<NetworkManager>();
			}
			else if (mgr == null)
				mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

			// If we are using the master server we need to get the registration data
			JSONNode masterServerData = null;
			if (!string.IsNullOrEmpty(Settings.masterServerHost))
			{
				masterServerData = mgr.MasterServerRegisterData(networker, Settings.serverId, Settings.serverName, Settings.type, Settings.mode, Settings.comment, Settings.useElo, Settings.eloRequired);
			}

			mgr.Initialize(networker, Settings.masterServerHost, Settings.masterServerPort, masterServerData);

			if (Settings.useInlineChat && networker.IsServer)
				SceneManager.sceneLoaded += CreateInlineChat;

			if (networker is IServer)
			{
				if (!Settings.DontChangeSceneOnConnect)
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
				else
					NetworkObject.Flush(networker); //Called because we are already in the correct scene!
			}
		}

		private void CreateInlineChat(Scene arg0, LoadSceneMode arg1)
		{
			SceneManager.sceneLoaded -= CreateInlineChat;
			var chat = NetworkManager.Instance.InstantiateChatManager();
			DontDestroyOnLoad(chat.gameObject);
		}

		private void SetToggledButtons(bool value)
		{
			for (int i = 0; i < _uiButtons.Count; ++i)
				_uiButtons[i].interactable = value;
		}

		private void OnApplicationQuit()
		{
			if (Settings.getLocalNetworkConnections)
				NetWorker.EndSession();

			if (server != null) server.Disconnect(true);
		}
	}
}

using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
	public InputField ipAddress = null;
	public InputField portNumber = null;
	public bool DontChangeSceneOnConnect = false;
	public string masterServerHost = string.Empty;
	public ushort masterServerPort = 15940;
	public string natServerHost = string.Empty;
	public ushort natServerPort = 15941;
	public bool connectUsingMatchmaking = false;
	public bool useElo = false;
	public int myElo = 0;
	public int eloRequired = 0;

	public GameObject networkManager = null;
	public GameObject[] ToggledButtons;
	private NetworkManager mgr = null;
	private NetWorker server;

	private List<Button> _uiButtons = new List<Button>();
	private bool _matchmaking = false;
	public bool useMainThreadManagerForRPCs = true;
	public bool useInlineChat = false;

	public bool getLocalNetworkConnections = false;

	public bool useTCP = false;

	private void Start()
	{
		ipAddress.text = "127.0.0.1";
		portNumber.text = "15937";

		for (int i = 0; i < ToggledButtons.Length; ++i)
		{
			Button btn = ToggledButtons[i].GetComponent<Button>();
			if (btn != null)
				_uiButtons.Add(btn);
		}

		if (!useTCP)
		{
			// Do any firewall opening requests on the operating system
			NetWorker.PingForFirewall(ushort.Parse(portNumber.text));
		}

		if (useMainThreadManagerForRPCs)
			Rpc.MainThreadRunner = MainThreadManager.Instance;

		if (getLocalNetworkConnections)
		{
			NetWorker.localServerLocated += LocalServerLocated;
			NetWorker.RefreshLocalUdpListings(ushort.Parse(portNumber.text));
		}
	}

	private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
	{
		Debug.Log("Found endpoint: " + endpoint.Address + ":" + endpoint.Port);
	}

	public void Connect()
	{
		if (connectUsingMatchmaking)
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

		if (useTCP)
		{
			client = new TCPClient();
			((TCPClient)client).Connect(ipAddress.text, (ushort)port);
		}
		else
		{
			client = new UDPClient();
			if (natServerHost.Trim().Length == 0)
				((UDPClient)client).Connect(ipAddress.text, (ushort)port);
			else
				((UDPClient)client).Connect(ipAddress.text, (ushort)port, natServerHost, natServerPort);
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

		mgr.MatchmakingServersFromMasterServer(masterServerHost, masterServerPort, myElo, (response) =>
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
		if (useTCP)
		{
			server = new TCPServer(64);
			((TCPServer)server).Connect();
		}
		else
		{
			server = new UDPServer(64);

			if (natServerHost.Trim().Length == 0)
				((UDPServer)server).Connect(ipAddress.text, ushort.Parse(portNumber.text));
			else
				((UDPServer)server).Connect(port: ushort.Parse(portNumber.text), natHost: natServerHost, natPort: natServerPort);
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
		if (!string.IsNullOrEmpty(masterServerHost))
		{
			string serverId = "myGame";
			string serverName = "Forge Game";
			string type = "Deathmatch";
			string mode = "Teams";
			string comment = "Demo comment...";

			masterServerData = mgr.MasterServerRegisterData(networker, serverId, serverName, type, mode, comment, useElo, eloRequired);
		}

		mgr.Initialize(networker, masterServerHost, masterServerPort, masterServerData);

		if (useInlineChat && networker.IsServer)
			SceneManager.sceneLoaded += CreateInlineChat;

		if (networker is IServer)
		{
			if (!DontChangeSceneOnConnect)
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
		if (getLocalNetworkConnections)
			NetWorker.EndSession();

		if (server != null) server.Disconnect(true);
	}
}

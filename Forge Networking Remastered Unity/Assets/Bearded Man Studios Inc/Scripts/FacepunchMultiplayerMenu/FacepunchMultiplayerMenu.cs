#if FACEPUNCH_STEAMWORKS
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Logging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;
using System.Threading.Tasks;


public class FacepunchMultiplayerMenu : MonoBehaviour
{
	public const int MAXIMUM_SERVER_SLOTS = 64;

	public bool dontChangeSceneOnConnect = false;
	public GameObject networkManager = null;
	public GameObject[] toggledButtons;
	public Steamworks.Data.Lobby lobbyToJoin = default;
	public bool useMainThreadManagerForRPCs = true;
	public bool useInlineChat = false;

	private NetworkManager mgr = null;
	private NetWorker server;
	private Steamworks.Data.Lobby lobby;
	private List<Button> _uiButtons = new List<Button>();

	private void Start()
	{
		for (int i = 0; i < toggledButtons.Length; ++i)
		{
			Button btn = toggledButtons[i].GetComponent<Button>();
			if (btn != null)
				_uiButtons.Add(btn);
		}

		if (useMainThreadManagerForRPCs)
			Rpc.MainThreadRunner = MainThreadManager.Instance;
	}

	/// <summary>
	/// Connects to the currently selected Steam Lobby
	/// Lobby selection is made within FacepunchJoinMenu.cs
	/// </summary>
	public void Connect()
	{
		if (lobbyToJoin.Id > 0)
		{
			ConnectToLobbyAsync();
		}
		else
		{
			BMSLog.LogWarning("No lobby selected to join");
		}
	}

	/// <summary>
	/// Connect to selected steam lobby asynchronously
	/// </summary>
	private async void ConnectToLobbyAsync()
	{
		await ConnectToLobby();
	}

	/// <summary>
	/// Connect to the selected steam lobby
	/// On successful connection, starts a new FacepunchP2PClient for connection to the lobby owner's FacepunchP2PServer
	/// </summary>
	private async Task ConnectToLobby()
	{
		RoomEnter roomEnter = await lobbyToJoin.Join();
		if (roomEnter != RoomEnter.Success)
		{
			BMSLog.Log("Error connecting to lobby returned: " + roomEnter.ToString());
			return;
		}

		this.lobby = lobbyToJoin;
		ConnectToServer(lobbyToJoin.Owner.Id);
	}

	/// <summary>
	/// Creates a new FacepunchP2PClient and attempts to connect to the specified SteamId
	/// </summary>
	/// <param name="hostSteamId">SteamId of the host/server</param>
	private void ConnectToServer(SteamId hostSteamId)
	{
		var client = new FacepunchP2PClient();
		client.Connect(hostSteamId);
		Connected(client);
	}

	/// <summary>
	/// Create a new Steam lobby and FacepunchP2PServer
	/// </summary>
	public void Host()
	{
		CreateLobbyAsync();
	}

	/// <summary>
	/// Creates steam lobby asynchronously
	/// </summary>
	private async void CreateLobbyAsync()
	{
		await CreateLobby();
	}

	/// <summary>
	/// Create steam lobby asynchronously
	/// Starts a new FacepunchP2PServer if lobby creation successful
	/// </summary>
	/// <returns></returns>
	private async Task CreateLobby()
	{
		Steamworks.Data.Lobby? lobby = await SteamMatchmaking.CreateLobbyAsync(MAXIMUM_SERVER_SLOTS);
		if (lobby.HasValue)
		{
			CreateFacepunchP2PServer();
		}

		// TODO:  Raise a dialog to inform of the failed lobby creation attempt
	}

	/// <summary>
	/// Create new FacepunchP2PServer
	/// </summary>
	private void CreateFacepunchP2PServer()
	{
		server = new FacepunchP2PServer(MAXIMUM_SERVER_SLOTS);
		((FacepunchP2PServer)server).Host();

		server.playerTimeout += (player, sender) =>
		{
			BMSLog.Log("Player " + player.NetworkId + " timed out");
		};

		Connected(server);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
			Host();
		else if (Input.GetKeyDown(KeyCode.C))
			Connect();
	}

	/// <summary>
	/// Finalize networker creation
	/// </summary>
	/// <param name="networker">The FacepunchP2PServer or FacepunchP2PClient just created</param>
	public void Connected(NetWorker networker)
	{
		if (!networker.IsBound)
		{
			BMSLog.LogWarning("NetWorker failed to bind");
			return;
		}

		if (mgr == null && networkManager == null)
		{
			BMSLog.LogWarning("A network manager was not provided, generating a new one instead");
			networkManager = new GameObject("Network Manager");
			mgr = networkManager.AddComponent<NetworkManager>();
		}
		else if (mgr == null)
			mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

		mgr.Initialize(networker);

		if (useInlineChat && networker.IsServer)
			SceneManager.sceneLoaded += CreateInlineChat;

		if (networker is IServer)
		{
			if (!dontChangeSceneOnConnect)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			else
				NetworkObject.Flush(networker);
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
		if (server != null)
			server.Disconnect(true);

		if (lobby.Id > 0)
			lobby.Leave();
	}

}
#endif

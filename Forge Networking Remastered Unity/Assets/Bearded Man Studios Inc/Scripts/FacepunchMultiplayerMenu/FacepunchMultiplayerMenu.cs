#if FACEPUNCH_STEAMWORKS
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;
using System.Threading.Tasks;


public class FacepunchMultiplayerMenu : MonoBehaviour
{
	public bool DontChangeSceneOnConnect = false;

	public GameObject networkManager = null;
	public GameObject[] ToggledButtons;
	private NetworkManager mgr = null;
	private NetWorker server;
	public const int MAXIMUM_SERVER_SLOTS = 64;
	private Steamworks.Data.Lobby lobby;
	public Steamworks.Data.Lobby lobbyToJoin = default;

	private List<Button> _uiButtons = new List<Button>();
	public bool useMainThreadManagerForRPCs = true;
	public bool useInlineChat = false;

	private void Start()
	{

		for (int i = 0; i < ToggledButtons.Length; ++i)
		{
			Button btn = ToggledButtons[i].GetComponent<Button>();
			if (btn != null)
				_uiButtons.Add(btn);
		}

		if (useMainThreadManagerForRPCs)
			Rpc.MainThreadRunner = MainThreadManager.Instance;

	}

	public void Connect()
	{
		if (lobbyToJoin.Id > 0)
		{
			Debug.Log("Attempting connection to steam lobby with steamId: " + lobbyToJoin.Id.Value);
			ConnectToLobbyAsync();
		}
		else
		{
			Debug.Log("No lobby selected to join");
		}
	}

	private async void ConnectToLobbyAsync()
	{
		await ConnectToLobby();
	}

	private async Task ConnectToLobby()
	{
		RoomEnter x = await lobbyToJoin.Join();
		if (x != RoomEnter.Success)
		{
			BeardedManStudios.Forge.Logging.BMSLog.Log("Error connecting to lobby returned: " + x.ToString());
			return;
		}
		this.lobby = lobbyToJoin;
		BeardedManStudios.Forge.Logging.BMSLog.Log("Connected to lobby, owner.Id = " + lobbyToJoin.Owner.Id.Value);
		ConnectToServer(lobbyToJoin.Owner.Id);
	}

	private void ConnectToServer(SteamId hostSteamId)
	{
		var client = new FacepunchP2PClient();
		client.Connect(hostSteamId);
		Connected(client);
	}

	public void Host()
	{
		CreateLobbyAsync();
	}

	private async void CreateLobbyAsync()
	{
		await CreateLobby();
	}

	private async Task CreateLobby()
	{
		Steamworks.Data.Lobby? lobby = await SteamMatchmaking.CreateLobbyAsync(MAXIMUM_SERVER_SLOTS);
		if (lobby.HasValue)
		{
			//Debug.Log("Steam lobby created successfully. Lobby ID: " + lobby.Value.Id);
			BeardedManStudios.Forge.Logging.BMSLog.Log("Steam lobby created successfully. Lobby ID: " + lobby.Value.Id);
			CreateFacepunchP2PServer();
		}
		else
		{
			BeardedManStudios.Forge.Logging.BMSLog.Log("Failed to create lobby");
		}

	}

	private void CreateFacepunchP2PServer()
	{
		server = new FacepunchP2PServer(MAXIMUM_SERVER_SLOTS);
		((FacepunchP2PServer)server).Host();

		server.playerTimeout += (player, sender) =>
		{
			Debug.Log("Player " + player.NetworkId + " timed out");
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

		mgr.Initialize(networker);

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
		if (server != null)
			server.Disconnect(true);
		if (lobby.Id > 0)
			lobby.Leave();
	}

}
#endif

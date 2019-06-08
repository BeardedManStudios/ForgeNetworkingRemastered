#if FACEPUNCH_STEAMWORKS
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;

using Steamworks;

public class FacepunchJoinMenu : MonoBehaviour
{
	public ScrollRect servers;
	public FacepunchServerListEntry serverListEntryTemplate;
	public RectTransform serverListContentRect;
	public Button connectButton;
	public Text selectedLobbyText;

	private const string LOBBY_TEXT = "Selected Lobby SteamId: ";

	private int selectedServer = -1;
	private List<FacepunchServerListItemData> serverList = new List<FacepunchServerListItemData>();
	private float serverListEntryTemplateHeight;
	private float nextListUpdateTime = 0f;
	private FacepunchMultiplayerMenu mpMenu;

	private void Start()
	{
		// Init the MainThreadManager
		MainThreadManager.Create();

		mpMenu = this.GetComponentInParent<FacepunchMultiplayerMenu>();
		serverListEntryTemplateHeight = ((RectTransform)serverListEntryTemplate.transform).rect.height;

		RefreshLobbyListAsync();
	}

	private void Update()
	{
		if (Time.time > nextListUpdateTime)
		{
			RefreshLobbyListAsync();
			nextListUpdateTime = Time.time + 5.0f + UnityEngine.Random.Range(0.0f, 1.0f);
		}
	}

	/// <summary>
	/// Called when a server list item is clicked. It will automatically connect on double click.
	/// </summary>
	/// <param name="e"></param>
	public void OnServerItemPointerClick(BaseEventData e)
	{
		var eventData = (PointerEventData)e;
		for (int i = 0; i < serverList.Count; ++i)
		{
			if (serverList[i].ListItem.gameObject != eventData.pointerPress)
				continue;

			SetSelectedServer(i);
			if (eventData.clickCount == 2)
				mpMenu.Connect();

			return;
		}
	}

	/// <summary>
	/// Add a server to the list of servers
	/// </summary>
	/// <param name="address"></param>
	/// <param name="port"></param>
	private void AddServer(Steamworks.Data.Lobby lobby)
	{
		var hostName = lobby.Id.Value.ToString();

		for (int i = 0; i < serverList.Count; ++i)
		{
			var server = serverList[i];
			if (server.Hostname == hostName)
			{
				// Already have that server listed nothing else to do
				return;
			}
		}

		var serverListItemData = new FacepunchServerListItemData {
			ListItem = GameObject.Instantiate<FacepunchServerListEntry>(serverListEntryTemplate, servers.content),
			Hostname = hostName,
		};
		serverListItemData.ListItem.gameObject.SetActive(true);

		serverListItemData.lobby = lobby;

		UpdateItem(serverListItemData);
		serverListItemData.NextUpdate = Time.time + 5.0f + UnityEngine.Random.Range(0.0f, 1.0f);

		serverList.Add(serverListItemData);
		SetListItemSelected(serverListItemData, false);

		RepositionItems();
	}

	/// <summary>
	/// Remove a server from the list
	/// </summary>
	/// <param name="index"></param>
	private void RemoveServer(int index)
	{
		var o = serverList[index];
		RemoveServer(o);
	}

	private void RemoveServer(FacepunchServerListItemData item)
	{
		Destroy(item.ListItem.gameObject);
		serverList.Remove(item);
		RepositionItems();
	}

	/// <summary>
	/// Reposition the server list items after a add/remove operation
	/// </summary>
	private void RepositionItems()
	{
		for (int i = 0; i < serverList.Count; i++)
		{
			PositionItem(serverList[i].ListItem.gameObject, i);
		}

		var sizeDelta = serverListContentRect.sizeDelta;
		sizeDelta.y = serverList.Count * serverListEntryTemplateHeight;
		serverListContentRect.sizeDelta = sizeDelta;
	}

	/// <summary>
	/// Set the position of an item in the server list
	/// </summary>
	/// <param name="item"></param>
	/// <param name="index"></param>
	private void PositionItem(GameObject item, int index)
	{
		var rectTransform = (RectTransform)item.transform;
		rectTransform.localPosition = new Vector3(0.0f, -serverListEntryTemplateHeight * index, 0.0f);
	}

	/// <summary>
	/// Select a server in the list and prefill the ipaddress and port fields
	/// </summary>
	/// <param name="index"></param>
	private void SetSelectedServer(int index)
	{
		if (selectedServer == index)
			return;

		selectedServer = index;
		for (int i = 0; i < serverList.Count; i++)
		{
			SetListItemSelected(serverList[i], index == i);
		}

		if (index >= 0)
		{
			selectedLobbyText.text = LOBBY_TEXT + serverList[index].Hostname;
			mpMenu.lobbyToJoin = serverList[index].lobby;
		}
		else
		{
			selectedLobbyText.text = LOBBY_TEXT + "0";
			mpMenu.lobbyToJoin = default;
		}
	}

	/// <summary>
	/// Set the border around the selected server entry
	/// </summary>
	/// <param name="data"></param>
	/// <param name="selected"></param>
	private void SetListItemSelected(FacepunchServerListItemData data, bool selected)
	{
		data.ListItem.GetComponent<Image>().enabled = selected;
	}

	/// <summary>
	/// Update a specific server's details on the server list.
	/// </summary>
	/// <param name="option">The server display information to update</param>
	private void UpdateItem(FacepunchServerListItemData option)
	{
		option.ListItem.hostName.text = option.Hostname;

	}

	private async void RefreshLobbyListAsync()
	{
		await RefreshLobbyList();
	}

	private async Task RefreshLobbyList()
	{
		var query = new Steamworks.Data.LobbyQuery();
		Steamworks.Data.Lobby[] lobbylist = await query.RequestAsync();
		if (lobbylist != null)
		{
			if (lobbylist.Length > 0)
			{
				for (int i = 0; i < lobbylist.Length; i++)
				{
					bool haveThisServer = false;
					for (int j = 0; j < serverList.Count; j++)
					{
						FacepunchServerListItemData data = serverList[j];
						if (data.lobby.Id == lobbylist[i].Id)
						{
							haveThisServer = true;
							UpdateItem(data);
							continue;
						}
					}
					if (haveThisServer)
						continue;
					AddServer(lobbylist[i]);
				}
			}
		}
	}
}

internal class FacepunchServerListItemData
{
	public string Hostname;
	public FacepunchServerListEntry ListItem;
	public float NextUpdate;
	public Steamworks.Data.Lobby lobby;
}
#endif

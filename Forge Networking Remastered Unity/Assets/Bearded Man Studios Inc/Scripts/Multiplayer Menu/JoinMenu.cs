using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.SQP;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeardedManStudios.MultiplayerMenu
{
	public class JoinMenu : MonoBehaviour
	{
		public ForgeSettings Settings;
		public ScrollRect servers;
		public ServerListEntry serverListEntryTemplate;
		public RectTransform serverListContentRect;
		public Button connectButton;
		public InputField serverAddress;
		public InputField serverPort;

		private int selectedServer = -1;
		private List<ServerListItemData> serverList = new List<ServerListItemData>();
		private float serverListEntryTemplateHeight;
		private float nextListUpdateTime = 0f;
		private MultiplayerMenu mpMenu;
		private SQPClient sqpClient;

		private void Awake()
		{
			// Init the MainThreadManager
			MainThreadManager.Create();

			mpMenu = this.GetComponentInParent<MultiplayerMenu>();
			Settings = mpMenu.Settings;
			sqpClient = new SQPClient();

			if (Settings.getLocalNetworkConnections)
			{
				NetWorker.localServerLocated += LocalServerLocated;
				NetWorker.RefreshLocalUdpListings();
			}

			serverListEntryTemplateHeight = ((RectTransform) serverListEntryTemplate.transform).rect.height;
		}

		private void Update()
		{
			if (Time.time > nextListUpdateTime)
			{
				NetWorker.RefreshLocalUdpListings();
				nextListUpdateTime = Time.time + 5.0f + UnityEngine.Random.Range(0.0f, 1.0f);
			}

			if (sqpClient != null)
			{
				sqpClient.Update();

				foreach (var server in serverList)
				{
					UpdateItem(server);
					if (Time.time > server.NextUpdate && server.SqpQuery.State == ClientState.Idle) {
						sqpClient.SendChallengeRequest(server.SqpQuery);
						server.NextUpdate = Time.time + 5.0f + UnityEngine.Random.Range(0.0f, 1.0f);
					}
				}
			}

		}

		private void OnDestroy()
		{
			if (sqpClient != null)
			{
				sqpClient.ShutDown();
				NetWorker.EndSession();
			}
		}

		/// <summary>
		/// Called when a server list item is clicked. It will automatically connect on double click.
		/// </summary>
		/// <param name="e"></param>
		public void OnServerItemPointerClick(BaseEventData e)
		{
			var eventData = (PointerEventData)e;
			for (int i = 0; i < serverList.Count; ++i) {
				if (serverList[i].ListItem.gameObject != eventData.pointerPress) continue;

				SetSelectedServer(i);
				if (eventData.clickCount == 2)
					mpMenu.Connect();

				return;
			}
		}

		/// <summary>
		/// The local server lookup callback. Adds found servers to the server list.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="sender"></param>
		private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
		{
			MainThreadManager.Run(() =>
			{
				AddServer(endpoint.Address, endpoint.Port);
			});
		}

		/// <summary>
		/// Add a server to the list of servers
		/// </summary>
		/// <param name="address"></param>
		/// <param name="port"></param>
		private void AddServer(string address, ushort port = NetWorker.DEFAULT_PORT)
		{
			var hostAndPort = $"{address}:{port}";

			for (int i = 0; i < serverList.Count; ++i)
			{
				var server = serverList[i];
				if (server.Hostname == hostAndPort)
				{
					// Already have that server listed nothing else to do
					return;
				}
			}

			var serverListItemData = new ServerListItemData {
				ListItem = GameObject.Instantiate<ServerListEntry>(serverListEntryTemplate, servers.content),
				Hostname = hostAndPort
			};
			serverListItemData.ListItem.gameObject.SetActive(true);

			var endpoint = NetWorker.ResolveHost(address, Settings.SQPPort);

			serverListItemData.SqpQuery = sqpClient.GetQuery(endpoint);

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
			Destroy(o.ListItem.gameObject);
			serverList.RemoveAt(index);
			RepositionItems();

		}

		/// <summary>
		/// Reposition the server list items after a add/remove operation
		/// </summary>
		private void RepositionItems()
		{
			for (int i = 0; i < serverList.Count; i++) {
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

			for (int i = 0; i < serverList.Count; i++) {
				SetListItemSelected(serverList[i], index == i);
			}

			if (index >= 0) {
				var addressParts = serverList[index].Hostname.Split(':');
				serverAddress.text = addressParts[0];

				if (addressParts.Length == 2)
					serverPort.text = addressParts[1];
				else
					serverPort.text = NetWorker.DEFAULT_PORT.ToString();
			} else
			{
				serverAddress.text = "";
				serverPort.text = NetWorker.DEFAULT_PORT.ToString();
			}
		}

		/// <summary>
		/// Set the border around the selected server entry
		/// </summary>
		/// <param name="data"></param>
		/// <param name="selected"></param>
		private void SetListItemSelected(ServerListItemData data, bool selected)
		{
			data.ListItem.GetComponent<Image>().enabled = selected;
		}

		/// <summary>
		/// Update a specific server's details on the server list.
		/// </summary>
		/// <param name="option">The server display information to update</param>
		private void UpdateItem(ServerListItemData option)
		{
			option.ListItem.hostName.text = option.Hostname;

			if (option.SqpQuery.ValidResult) {
				var sid = option.SqpQuery.ServerInfo.ServerInfoData;
				option.ListItem.serverName.text = sid.ServerName ?? "";
				option.ListItem.playerCount.text = $"{(int)sid.CurrentPlayers}/{(int)sid.MaxPlayers}";
				option.ListItem.pingTime.text = $"{(int)option.SqpQuery.RTT} ms";

			} else {
				option.ListItem.serverName.text = "Server offline";
				option.ListItem.playerCount.text = "-/-";
				option.ListItem.pingTime.text = "--";
			}
		}
	}

	class ServerListItemData
	{
		public string Hostname;
		public ServerListEntry ListItem;
		public float NextUpdate;
		public Query SqpQuery;
	}
}

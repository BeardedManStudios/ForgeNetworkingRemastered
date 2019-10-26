using System;
using System.Collections.Generic;
using BeardedManStudios.Forge.Logging;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.SQP;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TextFrame = BeardedManStudios.Forge.Networking.Frame.Text;

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
		private bool masterServerEnabled;
		private bool localDiscoveryEnabled;
		private TCPClient masterClient;

		private void Awake()
		{
			// Init the MainThreadManager
			MainThreadManager.Create();

			mpMenu = this.GetComponentInParent<MultiplayerMenu>();
			Settings = mpMenu.Settings;
			serverListEntryTemplateHeight = ((RectTransform) serverListEntryTemplate.transform).rect.height;

			masterServerEnabled = !string.IsNullOrEmpty(Settings.masterServerHost);
			localDiscoveryEnabled = Settings.getLocalNetworkConnections;

			// No need to do anything else if local discovery or master server is not enabled
			if (!localDiscoveryEnabled && !masterServerEnabled)
				return;

			sqpClient = new SQPClient();

			if (localDiscoveryEnabled)
			{
				NetWorker.localServerLocated += LocalServerLocated;
				NetWorker.RefreshLocalUdpListings();
			}

			if (masterServerEnabled)
				RefreshMasterServerListings();
		}

		private void Update()
		{
			// No need to do anything if local discovery or master server is not enabled
			if (!masterServerEnabled && !localDiscoveryEnabled)
				return;

			if (Time.time > nextListUpdateTime)
			{
				if (localDiscoveryEnabled)
					NetWorker.RefreshLocalUdpListings();

				if (masterServerEnabled)
					RefreshMasterServerListings();

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
		private void AddServer(string address, ushort port = NetWorker.DEFAULT_PORT, bool isLocal = true)
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
				Hostname = hostAndPort,
				IsLocal = isLocal
			};
			serverListItemData.ListItem.gameObject.SetActive(true);

			var endpoint = HostResolver.Resolve(address, Settings.SQPPort);

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
			RemoveServer(o);
		}

		private void RemoveServer(ServerListItemData item)
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

			if (option.SqpQuery.ValidResult)
			{
				var sid = option.SqpQuery.ServerInfo.ServerInfoData;
				option.ListItem.serverName.text = $"{sid.ServerName} ({option.LocalOrGlobal})";
				option.ListItem.playerCount.text = $"{sid.CurrentPlayers.ToString()}/{sid.MaxPlayers.ToString()}";
				option.ListItem.pingTime.text = $"{option.SqpQuery.RTT.ToString()} ms";
			}
			else
			{
				option.ListItem.serverName.text = "Server offline";
				option.ListItem.playerCount.text = "-/-";
				option.ListItem.pingTime.text = "--";
			}
		}

		private void RefreshMasterServerListings()
		{
			masterClient = new TCPMasterClient();

			masterClient.serverAccepted += (sender) =>
			{
				try
				{
					// Create the get request with the desired filters
					JSONNode sendData = JSONNode.Parse("{}");
					JSONClass getData = new JSONClass();
					getData.Add("id", Settings.serverId);
					getData.Add("type", Settings.type);
					getData.Add("mode", Settings.mode);

					sendData.Add("get", getData);

					// Send the request to the server
					masterClient.Send(TextFrame.CreateFromString(masterClient.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
				}
				catch
				{
					// If anything fails, then this client needs to be disconnected
					masterClient.Disconnect(true);
					masterClient = null;
				}
			};

			masterClient.textMessageReceived += (player, frame, sender) =>
			{
				try
				{
					JSONNode data = JSONNode.Parse(frame.ToString());
					if (data["hosts"] != null)
					{
						var response = new MasterServerResponse(data["hosts"].AsArray);

						if (response != null && response.serverResponse.Count > 0)
						{
							// Go through all of the available hosts and add them to the server browser
							foreach (var server in response.serverResponse)
							{
								// Run on main thread as we are creating UnityEngine.GameObjects
								MainThreadManager.Run(() => AddServer(server.Address, server.Port));
							}
						}
					}
				} finally
				{
					if (masterClient != null)
					{
						// If we succeed or fail the client needs to disconnect from the Master Server
						masterClient.Disconnect(true);
						masterClient = null;
					}
				}
			};

			masterClient.Connect(Settings.masterServerHost, Settings.masterServerPort);
		}
	}

	internal class ServerListItemData
	{
		public string Hostname;
		public ServerListEntry ListItem;
		public float NextUpdate;
		public Query SqpQuery;
		public bool IsLocal;

		public string LocalOrGlobal => IsLocal ? "LAN" : "Internet";
	}
}

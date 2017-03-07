﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Lobby;
using System;

namespace BeardedManStudios.Forge.Networking.Unity.Lobby
{
	public class LobbyManager : MonoBehaviour, ILobbyMaster
	{
		public GameObject PlayerItem;
		public LobbyPlayerItem Myself;
		public InputField ChatInputBox;
		public Text Chatbox;
		public Transform Grid;

		private const int BUFFER_PLAYER_ITEMS = 10;
		private List<LobbyPlayerItem> _lobbyPlayersInactive = new List<LobbyPlayerItem>();
		private List<LobbyPlayerItem> _lobbyPlayersPool = new List<LobbyPlayerItem>();
		private LobbyPlayer _myself;

		#region Interface Members
		private List<IClientMockPlayer> _lobbyPlayers = new List<IClientMockPlayer>();
		public List<IClientMockPlayer> LobbyPlayers
		{
			get
			{
				return _lobbyPlayers;
			}
		}

		private Dictionary<uint, IClientMockPlayer> _lobbyPlayersMap = new Dictionary<uint, IClientMockPlayer>();
		public Dictionary<uint, IClientMockPlayer> LobbyPlayersMap
		{
			get
			{
				return _lobbyPlayersMap;
			}
		}

		private Dictionary<int, List<IClientMockPlayer>> _lobbyTeams = new Dictionary<int, List<IClientMockPlayer>>();
		public Dictionary<int, List<IClientMockPlayer>> LobbyTeams
		{
			get
			{
				return _lobbyTeams;
			}
		}
		#endregion

		public void Awake()
		{
			if (NetworkManager.Instance.IsServer)
			{
				SetupComplete();
				return;
			}

			NetworkObject.objectCreateRequested += CheckForService;

			for (int i = 0; i < NetworkManager.Instance.Networker.NetworkObjectList.Count; ++i)
			{
				NetworkObject n = NetworkManager.Instance.Networker.NetworkObjectList[i];
				if (n is LobbyService.LobbyServiceNetworkObject)
				{
					NetworkObject.objectCreateRequested -= CheckForService;
					SetupService(n);
					break;
				}
			}
		}

		private void CheckForService(NetWorker networker, int identity, uint id, Frame.FrameStream frame, Action<NetworkObject> callback)
		{
			if (identity != LobbyService.LobbyServiceNetworkObject.IDENTITY)
			{
				return;
			}

			NetworkObject obj = new LobbyService.LobbyServiceNetworkObject(networker, id, frame);
			if (callback != null)
				callback(obj);

			//Debug.Log("NOT COOL KIDS: " + obj.GetType().ToString());
			//if (!(obj is LobbyService.LobbyServiceNetworkObject))
			//	return;

			SetupService(obj);
		}

		private void SetupService(NetworkObject obj)
		{
			LobbyService.Instance.Initialize(obj);
			SetupComplete();
		}

		#region Public API
		public void ChangeName(LobbyPlayerItem item, string newName)
		{
			LobbyService.Instance.SetName(newName);
		}

		public void KickPlayer(LobbyPlayerItem item)
		{
			LobbyPlayer playerKicked = item.AssociatedPlayer;
			LobbyService.Instance.KickPlayer(playerKicked.NetworkId);
		}

		public void ChangeAvatarID(LobbyPlayerItem item, int nextID)
		{
			LobbyService.Instance.SetAvatar(nextID);
		}

		public void ChangeTeam(LobbyPlayerItem item, int nextTeam)
		{
			LobbyService.Instance.SetTeamId(nextTeam);
		}

		public void SendPlayersMessage()
		{
			string chatMessage = ChatInputBox.text;
			if (string.IsNullOrEmpty(chatMessage))
				return;

			LobbyService.Instance.SendPlayerMessage(chatMessage);
			ChatInputBox.text = string.Empty;
		}
		#endregion

		#region Private API
		private LobbyPlayerItem GetNewPlayerItem()
		{
			LobbyPlayerItem returnValue = null;

			if (_lobbyPlayersInactive.Count > 0)
			{
				returnValue = _lobbyPlayersInactive[0];
				_lobbyPlayersInactive.Remove(returnValue);
				_lobbyPlayersPool.Add(returnValue);
			}
			else
			{
				//Generate more!
				for (int i = 0; i < BUFFER_PLAYER_ITEMS - 1; ++i)
				{
					LobbyPlayerItem item = CreateNewPlayerItem();
					item.ToggleObject(false);
					item.SetParent(Grid);
					_lobbyPlayersInactive.Add(item);
				}
				returnValue = CreateNewPlayerItem();
			}
			returnValue.ToggleObject(true);

			return returnValue;
		}

		private void PutBackToPool(LobbyPlayerItem item)
		{
			item.ToggleInteractables(false);
			item.ToggleObject(false);
			_lobbyPlayersPool.Remove(item);
			_lobbyPlayersInactive.Add(item);
		}

		private LobbyPlayerItem CreateNewPlayerItem()
		{
			LobbyPlayerItem returnValue = Instantiate(PlayerItem).GetComponent<LobbyPlayerItem>();
			returnValue.Init(this);
			return returnValue;
		}

		private LobbyPlayerItem GrabLobbyPlayerItem(IClientMockPlayer player)
		{
			LobbyPlayerItem returnValue = null;

			for (int i = 0; i < _lobbyPlayersPool.Count; ++i)
			{
				if (_lobbyPlayersPool[i].AssociatedPlayer == player)
				{
					returnValue = _lobbyPlayersPool[i];
				}
			}

			return returnValue;
		}

		private LobbyPlayer GrabPlayer(IClientMockPlayer player)
		{
			LobbyPlayer returnValue = player as LobbyPlayer;
			if (returnValue == null)
			{
				for (int i = 0; i < LobbyPlayers.Count; ++i)
				{
					if (LobbyPlayers[i].NetworkId == player.NetworkId)
					{
						returnValue = LobbyPlayers[i] as LobbyPlayer;
						break;
					}
				}

				if (returnValue == null)
				{
					returnValue = new LobbyPlayer();
					returnValue.Name = player.Name;
					returnValue.NetworkId = player.NetworkId;
				}
			}

			return returnValue;
		}
		#endregion

		#region Interface API
		public void OnFNPlayerConnected(IClientMockPlayer player)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			MainThreadManager.Run(() =>
			{
				if (!LobbyPlayers.Contains(convertedPlayer))
				{
					Debug.Log("Doesn't contain: " + player.NetworkId);
					_lobbyPlayers.Add(convertedPlayer);
					_lobbyPlayersMap.Add(convertedPlayer.NetworkId, convertedPlayer);
					OnFNTeamChanged(convertedPlayer, 0);
					LobbyPlayerItem item = GetNewPlayerItem();
					//TODO: Replace isServer with an actual user check
					item.Setup(convertedPlayer, LobbyService.Instance.IsServer);
					if (LobbyService.Instance.IsServer)
						item.KickButton.SetActive(true);
					item.SetParent(Grid);
				}
				else
					Debug.Log("Already contains: " + player.NetworkId);

			});
		}

		public void OnFNPlayerDisconnected(IClientMockPlayer player)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			MainThreadManager.Run(() =>
			{
				if (!LobbyPlayers.Contains(convertedPlayer))
				{
					_lobbyPlayers.Add(convertedPlayer);
					_lobbyPlayersMap.Add(convertedPlayer.NetworkId, convertedPlayer);
					OnFNTeamChanged(convertedPlayer, 0);

					LobbyPlayerItem item = GrabLobbyPlayerItem(convertedPlayer);
					if (item != null)
						PutBackToPool(item);
				}
			});
		}

		public void OnFNPlayerNameChanged(IClientMockPlayer player)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			convertedPlayer.Name = player.Name;
			if (_myself == convertedPlayer)
				Myself.ChangeName(convertedPlayer.Name);
			else
			{
				LobbyPlayerItem item = GrabLobbyPlayerItem(convertedPlayer);
				item.ChangeName(convertedPlayer.Name);
			}
		}

		public void OnFNTeamChanged(IClientMockPlayer player, int newID)
		{
			if (!LobbyTeams.ContainsKey(newID))
				LobbyTeams.Add(newID, new List<IClientMockPlayer>());

			//We do this to not make Foreach loops
			IEnumerator iter = LobbyTeams.GetEnumerator();
			iter.Reset();
			while (iter.MoveNext())
			{
				if (iter.Current != null)
				{
					KeyValuePair<int, List<IClientMockPlayer>> kv = (KeyValuePair<int, List<IClientMockPlayer>>)iter.Current;
					if (kv.Value.Contains(player))
					{
						kv.Value.Remove(player);
						break;
					}
				}
				else
					break;
			}

			//We prevent the player being added twice to the same team
			if (!LobbyTeams[newID].Contains(player))
			{
				LobbyPlayer convertedPlayer = player as LobbyPlayer;
				convertedPlayer.TeamID = newID;

				if (_myself == convertedPlayer)
					Myself.ChangeTeam(newID);
				else
				{
					LobbyPlayerItem item = GrabLobbyPlayerItem(convertedPlayer);
					if (item != null)
						item.ChangeTeam(newID);
				}

				LobbyTeams[newID].Add(player);
			}
		}

		public void OnFNAvatarIDChanged(IClientMockPlayer player, int newID)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			convertedPlayer.AvatarID = newID;
			if (_myself == convertedPlayer)
				Myself.ChangeAvatarID(convertedPlayer.AvatarID);
			else
			{
				LobbyPlayerItem item = GrabLobbyPlayerItem(convertedPlayer);
				item.ChangeAvatarID(convertedPlayer.AvatarID);
			}
		}

		public void OnFNLobbyPlayerMessageReceived(IClientMockPlayer player, string message)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			Chatbox.text += string.Format("{0}: {1}\n", convertedPlayer.Name, message);
		}

		public void OnFNLobbyMasterKnowledgeTransfer(ILobbyMaster previousLobbyMaster)
		{
			LobbyPlayers.Clear();
			LobbyPlayersMap.Clear();
			LobbyTeams.Clear();
			for (int i = 0; i < previousLobbyMaster.LobbyPlayers.Count; ++i)
			{
				LobbyPlayer player = GrabPlayer(previousLobbyMaster.LobbyPlayers[i]);
				LobbyPlayers.Add(player);
				LobbyPlayersMap.Add(player.NetworkId, player);
			}

			IEnumerator iterTeams = previousLobbyMaster.LobbyTeams.GetEnumerator();
			iterTeams.Reset();
			while (iterTeams.MoveNext())
			{
				if (iterTeams.Current != null)
				{
					KeyValuePair<int, List<IClientMockPlayer>> kv = (KeyValuePair<int, List<IClientMockPlayer>>)iterTeams.Current;
					List<IClientMockPlayer> players = new List<IClientMockPlayer>();
					for (int i = 0; i < kv.Value.Count; ++i)
					{
						players.Add(GrabPlayer(kv.Value[i]));
					}
					LobbyTeams.Add(kv.Key, players);
				}
				else
					break;
			}

			IEnumerator iterPlayersMap = previousLobbyMaster.LobbyPlayersMap.GetEnumerator();
			iterPlayersMap.Reset();
			while (iterPlayersMap.MoveNext())
			{
				if (iterPlayersMap.Current != null)
				{
					KeyValuePair<uint, IClientMockPlayer> kv = (KeyValuePair<uint, IClientMockPlayer>)iterPlayersMap.Current;

					if (LobbyPlayersMap.ContainsKey(kv.Key))
						LobbyPlayersMap[kv.Key] = GrabPlayer(kv.Value);
					else
						LobbyPlayersMap.Add(kv.Key, GrabPlayer(kv.Value));
				}
				else
					break;
			}
		}

		private void SetupComplete()
		{
			LobbyService.Instance.SetLobbyMaster(this);

			if (LobbyService.Instance.IsServer)
				LobbyService.Instance.PlayerConnected(LobbyService.Instance.MyMockPlayer);
			//If I am the host, then I should show the kick button for all players here
			LobbyPlayerItem item = GetNewPlayerItem(); //This will just auto generate the 10 items we need to start with
			item.SetParent(Grid);
			PutBackToPool(item);

			_myself = GrabPlayer(LobbyService.Instance.MyMockPlayer);
			if (!LobbyPlayers.Contains(_myself))
				LobbyPlayers.Add(_myself);
			Myself.Init(this);
			Myself.Setup(_myself, true);

			List<IClientMockPlayer> currentPlayers = LobbyService.Instance.MasterLobby.LobbyPlayers;
			for (int i = 0; i < currentPlayers.Count; ++i)
			{
				IClientMockPlayer currentPlayer = currentPlayers[i];
				if (currentPlayer == _myself)
					continue;
				OnFNPlayerConnected(currentPlayers[i]);
			}
		}
		#endregion
	}
}
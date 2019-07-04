using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Lobby;
using System;
using UnityEngine.SceneManagement;

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
		private NetworkObject _networkObjectReference;

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


			for (int i = 0; i < NetworkObject.NetworkObjects.Count; ++i)
			{
				NetworkObject n = NetworkObject.NetworkObjects[i];
				if (n is LobbyService.LobbyServiceNetworkObject)
				{
					SetupService(n);
					return;
				}
			}

            NetworkManager.Instance.Networker.objectCreateRequested += CheckForService;
            NetworkManager.Instance.Networker.factoryObjectCreated += FactoryObjectCreated;
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
			SetupService(obj);
		}

		private void FactoryObjectCreated(NetworkObject obj)
		{
			if (obj.UniqueIdentity != LobbyService.LobbyServiceNetworkObject.IDENTITY)
				return;

            NetworkManager.Instance.Networker.factoryObjectCreated -= FactoryObjectCreated;
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

        public void StartGame(int sceneID)
        {
#if UNITY_5_6_OR_NEWER
            SceneManager.LoadScene(sceneID);
#else
            Application.LoadLevel(sceneID);
#endif
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
				_lobbyPlayersPool.Add(returnValue);
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
				if (_lobbyPlayersPool[i].AssociatedPlayer.NetworkId == player.NetworkId)
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
						LobbyPlayer tPlayer = LobbyPlayers[i] as LobbyPlayer;
						if (tPlayer == null)
						{
							tPlayer = new LobbyPlayer();
							tPlayer.Name = player.Name;
							tPlayer.NetworkId = player.NetworkId;
                            tPlayer.AvatarID = player.AvatarID;
                            tPlayer.TeamID = player.TeamID;
							LobbyPlayers[i] = tPlayer;
						}
						returnValue = tPlayer;
						returnValue.Name = player.Name;
						break;
					}
				}

				if (returnValue == null)
				{
					returnValue = new LobbyPlayer();
					returnValue.Name = player.Name;
					returnValue.NetworkId = player.NetworkId;
                    returnValue.AvatarID = player.AvatarID;
                    returnValue.TeamID = player.TeamID;
				}
			}

			return returnValue;
		}
		#endregion

		#region Interface API
		public void OnFNPlayerConnected(IClientMockPlayer player)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			if (convertedPlayer == _myself || _myself == null)
				return; //Ignore re-adding ourselves

            bool playerCreated = false;
            for (int i = 0; i < _lobbyPlayersPool.Count; ++i)
            {
                if (_lobbyPlayersPool[i].AssociatedPlayer.NetworkId == player.NetworkId)
                    playerCreated = true;
            }

            playerCreated = convertedPlayer.Created;
            if (playerCreated)
                return;

            convertedPlayer.Created = true;

            if (!LobbyPlayers.Contains(convertedPlayer))
                _lobbyPlayers.Add(convertedPlayer);
            if (_lobbyPlayersMap.ContainsKey(convertedPlayer.NetworkId))
                _lobbyPlayersMap[convertedPlayer.NetworkId] = convertedPlayer;
            else
                _lobbyPlayersMap.Add(convertedPlayer.NetworkId, convertedPlayer);

            OnFNTeamChanged(convertedPlayer);

            MainThreadManager.Run(() =>
            {
                LobbyPlayerItem item = GetNewPlayerItem();
                item.Setup(convertedPlayer, false);
                if (LobbyService.Instance.IsServer)
                    item.KickButton.SetActive(true);
                item.SetParent(Grid);
            });
        }

		public void OnFNPlayerDisconnected(IClientMockPlayer player)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			MainThreadManager.Run(() =>
			{
				if (LobbyPlayers.Contains(convertedPlayer))
				{
					_lobbyPlayers.Remove(convertedPlayer);
					_lobbyPlayersMap.Remove(convertedPlayer.NetworkId);

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
                if (item != null)
                    item.ChangeName(convertedPlayer.Name);
			}
		}

		public void OnFNTeamChanged(IClientMockPlayer player)
		{
            int newID = player.TeamID;
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

		public void OnFNAvatarIDChanged(IClientMockPlayer player)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			convertedPlayer.AvatarID = player.AvatarID;
			if (_myself == convertedPlayer)
				Myself.ChangeAvatarID(convertedPlayer.AvatarID);
			else
			{
				LobbyPlayerItem item = GrabLobbyPlayerItem(convertedPlayer);
                if (item != null)
                    item.ChangeAvatarID(convertedPlayer.AvatarID);
			}
		}

		public void OnFNLobbyPlayerMessageReceived(IClientMockPlayer player, string message)
		{
			LobbyPlayer convertedPlayer = GrabPlayer(player);
			Chatbox.text += string.Format("{0}: {1}\n", convertedPlayer.Name, message);
		}

        public void OnFNPlayerSync(IClientMockPlayer player)
        {
            OnFNAvatarIDChanged(player);
            OnFNTeamChanged(player);
            OnFNPlayerNameChanged(player);
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
            LobbyService.Instance.Initialize(NetworkManager.Instance.Networker);
            
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
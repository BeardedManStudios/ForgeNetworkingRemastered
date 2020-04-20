using Forge.Factory;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using Forge.Unity;
using Puzzle.Networking.Messages;
using Puzzle.Networking.Messages.Interpreters;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzle.Network
{
	public class ComplexSampleNetwork : MonoBehaviour
	{
		private IUnityEntity _myPlayer = null;
		public IUnityEntity MyPlayer
		{
			get => _myPlayer;
			set
			{
				_myPlayer = value;
				SetPlayerStartPosition();
			}
		}

		[SerializeField] private int _playerPrefabId = 0;
		[SerializeField] private int _playerProxyPrefabId = 0;
		private int _entityId = 0;

		private IEngineFacade _engine = null;
		private MessagePool<SpawnEntityMessage> _msgPool = new MessagePool<SpawnEntityMessage>();

		private void Awake()
		{
			AbstractFactory.Register<IGameplayTypeFactory, GameplayTypeFactory>();

			DontDestroyOnLoad(transform.parent);
		}

		private void Start()
		{
			_engine = FindObjectOfType<ForgeEngineFacade>();
			SceneManager.sceneLoaded += SceneLoaded;
			if (_engine.IsServer)
			{
				CreatePlayerMessage msg = GetPlayerCreateMessage(_engine.NetworkMediator.SocketFacade.NetPlayerId, _entityId++);
				MyPlayer = EntitySpawner.SpawnEntityFromMessage(_engine, msg);    // Spawn for ourselves locally
				MyPlayer.PrefabId = _playerProxyPrefabId;  // This is a hack for when we send out all entities
				DontDestroyOnLoad(MyPlayer.OwnerGameObject);
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
			else
			{
				_engine.NetworkMediator.SendReliableMessage(new MapLoadRequestMessage());
				_engine.NetworkMediator.SendReliableMessage(new CreateMyPlayerMessage());
				CreatePlayerInterpreter.Instance.onPlayerCreated += OnPlayerCreated;
			}

		}

		private void OnPlayerCreated(IUnityEntity entity, IPlayerSignature owningPlayer)
		{
			if (_engine.NetworkMediator.SocketFacade.NetPlayerId.Equals(owningPlayer))
				GameObject.FindObjectOfType<ComplexSampleNetwork>().MyPlayer = entity;
			GameObject.DontDestroyOnLoad(entity.OwnerGameObject);
		}

		private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
		{
			SetPlayerStartPosition();
			if (!_engine.IsServer)
			{
				_engine.NetworkMediator.SendReliableMessage(new GetAllEntitiesRequestMessage());
				return;
			}
			var entities = UnityExtensions.FindInterfaces<IUnityEntity>();
			foreach (var e in entities)
			{
				if (_engine.EntityRepository.Exists(e)) continue;
				if (!string.IsNullOrEmpty(e.SceneIdentifier))
				{
					e.Id = _entityId++;
					_engine.EntityRepository.Add(e);
				}
			}
			_engine.NetworkMediator.SendReliableMessage(new MapChangedMessage { MapName = scene.name });
		}

		private void SetPlayerStartPosition()
		{
			if (MyPlayer == null) return;
			Transform st = GameObject.Find("Player Start Position")?.transform;
			Vector3 pos = Vector3.zero;
			if (st != null)
				pos = st.position;
			MyPlayer.OwnerGameObject.transform.position = pos;
		}

		public void PlayerJoined(INetPlayer player)
		{
			int entityId = _entityId++;
			CreatePlayerMessage msg = GetPlayerCreateMessage(player.Id, entityId);
			_engine.NetworkMediator.SendMessage(msg);

			// Spawn for ourselves locally
			CreatePlayerMessage localMsg = GetPlayerCreateMessage(player.Id, entityId);
			localMsg.PrefabId = _playerProxyPrefabId;
			EntitySpawner.SpawnEntityFromMessage(_engine, localMsg);
		}

		private CreatePlayerMessage GetPlayerCreateMessage(IPlayerSignature playerId, int entityId)
		{
			return new CreatePlayerMessage()
			{
				Id = entityId,
				PrefabId = _playerPrefabId,
				Position = Vector3.zero,
				Rotation = Quaternion.Euler(new Vector3(0.0f, 103.0f, 0.0f)),
				Scale = Vector3.one,
				ProxyPrefabId = _playerProxyPrefabId,
				OwningPlayer = playerId
			};
		}

		public void SpawnPrefab(int prefabId, Vector3 pos, Vector3 rot, Vector3 scale)
		{
			var msg = _msgPool.Get();
			msg.Id = _entityId++;
			msg.PrefabId = prefabId;
			msg.Position = pos;
			msg.Rotation = Quaternion.Euler(rot);
			msg.Scale = scale;
			EntitySpawner.SpawnEntityFromMessage(_engine, msg);    // Spawn for ourselves locally
			_engine.NetworkMediator.SendReliableMessage(msg);
		}

		public void SpawnRemotePrefab(int prefabId, Vector3 pos, Vector3 rot, Vector3 scale)
		{
			var msg = _msgPool.Get();
			msg.Id = _entityId++;
			msg.PrefabId = prefabId;
			msg.Position = pos;
			msg.Rotation = Quaternion.Euler(rot);
			msg.Scale = scale;
			_engine.NetworkMediator.SendReliableMessage(msg);
		}
	}
}

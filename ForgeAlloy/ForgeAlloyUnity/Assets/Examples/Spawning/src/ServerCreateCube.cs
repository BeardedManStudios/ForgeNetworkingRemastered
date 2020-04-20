using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using UnityEngine;

public class ServerCreateCube : MonoBehaviour
{
	[SerializeField]
	private int _prefabId = 0;

	private int _runningId = 0;
	private IEngineFacade _engine;
	private MessagePool<SpawnEntityMessage> _msgPool = new MessagePool<SpawnEntityMessage>();

	private void Awake()
	{
		_engine = GameObject.FindObjectOfType<ForgeEngineFacade>();
	}

	private void Update()
	{
		if (!_engine.IsServer)
			return;

		if (Input.GetKeyDown(KeyCode.B))
			SpawnOnServer();
	}

	private void SpawnOnServer()
	{
		var msg = _msgPool.Get();
		msg.Id = _runningId++;
		msg.PrefabId = _prefabId;
		msg.Position = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f));
		msg.Rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
		msg.Scale = new Vector3(Random.Range(0.5f, 3.0f), Random.Range(0.5f, 3.0f), Random.Range(0.5f, 3.0f));
		EntitySpawner.SpawnEntityFromMessage(_engine, msg);    // Spawn for ourselves locally
		_engine.NetworkMediator.SendMessage(msg);
	}
}

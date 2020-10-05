using Forge.Networking.Unity;
using UnityEngine;

public class DisconnectButton : MonoBehaviour
{
	private IEngineFacade _engine = null;

	private void Awake()
	{
		_engine = GameObject.FindObjectOfType<ForgeEngineFacade>();
	}

	public void Disconnect()
	{
		_engine.ShutDown();
	}
}

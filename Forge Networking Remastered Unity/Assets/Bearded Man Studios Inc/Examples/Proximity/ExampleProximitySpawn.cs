using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class ExampleProximitySpawn : MonoBehaviour
{
	private void Start()
	{
		NetworkManager.Instance.InstantiateExampleProximityPlayer();
	}
}
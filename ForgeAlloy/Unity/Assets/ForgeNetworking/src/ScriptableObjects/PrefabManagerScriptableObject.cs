using UnityEngine;

namespace Forge.Networking.Unity
{
	[CreateAssetMenu(fileName = "ForgeNetworkingExampleData",
		menuName = "Forge Networking/Scriptable Objects/PrefabManager", order = 1)]
	public class PrefabManagerScriptableObject : ScriptableObject, IPrefabManager
	{
		[SerializeField]
		private Transform[] _prefabs = new Transform[0];

		public Transform GetPrefabById(int id)
		{
			// TODO:  Return a prefab not found exception if not found
			return _prefabs[id];
		}
	}
}

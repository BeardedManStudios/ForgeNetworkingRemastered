using System;
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
			if (id < 0 || id >= _prefabs.Length)
			{
				throw new ArgumentException($"The id for the prefab lookup was invalid, it should be 0-{_prefabs.Length - 1}", nameof(id));
			}
			return _prefabs[id];
		}
	}
}

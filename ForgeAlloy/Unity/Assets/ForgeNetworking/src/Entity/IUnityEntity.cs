using UnityEngine;

namespace Forge.Networking.Unity
{
	public interface IUnityEntity
	{
		int Id { get; set; }
		int PrefabId { get; set; }
		GameObject OwnerGameObject { get; }
		string SceneIdentifier { get; set; }
	}
}

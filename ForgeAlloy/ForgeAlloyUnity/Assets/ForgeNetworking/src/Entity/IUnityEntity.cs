using Forge.Networking.Players;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public delegate void OwnedUnityEntityEvent(IUnityEntity entity, IPlayerSignature owningPlayer);

	public interface IUnityEntity
	{
		int Id { get; set; }
		int PrefabId { get; set; }
		GameObject OwnerGameObject { get; }
		int SceneIndex { get; }
		string SceneIdentifier { get; set; }
	}
}

using UnityEngine;

namespace Forge.Networking.Unity
{
	public interface IUnityEntity
	{
		int Id { get; set; }
		GameObject OwnerGameObject { get; }
	}
}

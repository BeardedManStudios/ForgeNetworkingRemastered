using UnityEngine;

namespace Forge.Networking.Unity
{
	public interface IUnityEntity
	{
		GameObject OwnerGameObject { get; }
	}
}

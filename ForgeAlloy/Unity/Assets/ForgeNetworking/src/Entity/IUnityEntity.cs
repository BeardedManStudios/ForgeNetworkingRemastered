using Forge.Engine;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public interface IUnityEntity : IEntity
	{
		GameObject OwnerGameObject { get; }
	}
}

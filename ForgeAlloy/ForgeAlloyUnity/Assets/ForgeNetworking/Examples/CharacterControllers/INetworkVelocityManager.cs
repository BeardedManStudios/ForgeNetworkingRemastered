using UnityEngine;

namespace Forge.CharacterControllers
{
	public interface INetworkTransformManager
	{
		void Initialize(Transform transform, Transform camTransform);
		void Process();
	}
}
using UnityEngine;

namespace Forge.CharacterControllers
{
	public interface IFriendPreviewManager
	{
		void Initialize(Transform target, RenderTexture texture);
		void Update();
	}
}
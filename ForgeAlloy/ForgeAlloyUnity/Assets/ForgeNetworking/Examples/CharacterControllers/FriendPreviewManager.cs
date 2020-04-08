using Forge.CharacterControllers.Network;
using Forge.Unity;
using System.Linq;
using UnityEngine;

namespace Forge.CharacterControllers
{
	public class FriendPreviewManager : IFriendPreviewManager
	{
		private Transform _transform = null;
		private RenderTexture _texture = null;
		private Camera _currentCam = null;
		private bool _showing = false;

		public void Initialize(Transform target, RenderTexture texture)
		{
			_transform = target;
			_texture = texture;
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (_showing)
					Hide();
				else
					Show();
			}
		}

		private void Show()
		{
			var t = UnityExtensions.FindInterfaces<IProxyPlayer>().FirstOrDefault();
			if (t == null) return;
			_currentCam = t.Camera;
			_currentCam.enabled = true;
			_currentCam.targetTexture = _texture;
			_transform.gameObject.SetActive(true);
			_showing = true;
		}

		private void Hide()
		{
			_transform.gameObject.SetActive(false);
			if (_currentCam == null) return;
			_currentCam.targetTexture = null;
			_currentCam.enabled = false;
			_currentCam = null;
		}
	}
}
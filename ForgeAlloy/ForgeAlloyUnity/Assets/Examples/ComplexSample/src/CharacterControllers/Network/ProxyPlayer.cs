using Forge.Factory;
using UnityEngine;

namespace Forge.CharacterControllers.Network
{
	public class ProxyPlayer : MonoBehaviour, IProxyPlayer
	{
		[SerializeField] private Camera _camera = null;

		public Camera Camera => _camera;

		private void Start()
		{
			var f = AbstractFactory.Get<IGameplayTypeFactory>();
			DontDestroyOnLoad(gameObject);
		}
	}
}

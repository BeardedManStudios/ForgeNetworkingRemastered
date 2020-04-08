using Forge.Factory;
using UnityEngine;

namespace Forge.CharacterControllers.Network
{
	public class ProxyPlayer : MonoBehaviour, IProxyPlayer
	{
		[SerializeField] private Camera _camera = null;
		[SerializeField] private GameObject _northFieldPrefab = null;
		[SerializeField] private GameObject _southFieldPrefab = null;

		public Camera Camera => _camera;

		private void Start()
		{
			var f = AbstractFactory.Get<IGameplayTypeFactory>();
			DontDestroyOnLoad(gameObject);
		}
	}
}

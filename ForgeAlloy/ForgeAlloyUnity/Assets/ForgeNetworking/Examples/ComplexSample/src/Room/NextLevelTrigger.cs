using Forge.CharacterControllers.Network;
using Forge.Networking.Unity;
using Forge.Unity;
using Puzzle.Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzle.Room
{
	public class NextLevelTrigger : MonoBehaviour
	{
		private IEngineFacade _engine;
		private List<IUnityEntity> _entities = new List<IUnityEntity>();
		private ComplexSampleNetwork _puzzle;

		private void Start()
		{
			_engine = UnityExtensions.FindInterface<IEngineFacade>();
			_puzzle = FindObjectOfType<ComplexSampleNetwork>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_engine.IsServer) return;
			var ue = other.gameObject.GetComponent<IUnityEntity>();
			if (ue == null) return;
			var proxy = other.gameObject.GetComponent<IProxyPlayer>();
			if (proxy != null)
				_entities.Add(ue);
			else if (ue == _puzzle.MyPlayer)
				_entities.Add(ue);
			var proxies = UnityExtensions.FindInterfaces<IProxyPlayer>();
			if (_entities.Count == proxies.Count + 1)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!_engine.IsServer) return;
			var ue = other.gameObject.GetComponent<IUnityEntity>();
			if (ue == null) return;
			_entities.Remove(ue);
		}
	}
}

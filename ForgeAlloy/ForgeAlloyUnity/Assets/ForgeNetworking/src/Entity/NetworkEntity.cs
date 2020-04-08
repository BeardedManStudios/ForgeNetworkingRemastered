using Forge.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity
{
	public class NetworkEntity : MonoBehaviour, IUnityEntity
	{
		[SerializeField] private string _sceneIdentifier = null;
		[SerializeField, HideInInspector] private int _sceneIndex = 0;
		public int Id { get; set; }
		public int PrefabId { get; set; }
		public GameObject OwnerGameObject => gameObject;
		public int SceneIndex => _sceneIndex;
		public string SceneIdentifier
		{
			get => _sceneIdentifier;
			set { _sceneIdentifier = value; }
		}

		private IEngineFacade _engine;

		private void Awake()
		{
			_sceneIndex = SceneManager.GetActiveScene().buildIndex;
		}

		private void Start()
		{
			_engine = UnityExtensions.FindInterface<IEngineFacade>();
		}

		private void OnDestroy()
		{
			_engine.EntityRepository.Remove(this);
		}

		private void OnValidate()
		{
			var entities = GameObject.FindObjectsOfType<NetworkEntity>();
			List<string> _currentIds = entities.Where(e => e != this).Select(e => e._sceneIdentifier).ToList();
			foreach (var e in entities)
			{
				if (e == this)
				{
					if (string.IsNullOrEmpty(_sceneIdentifier))
						_sceneIdentifier = Guid.NewGuid().ToString();
					else if (_currentIds.Contains(e._sceneIdentifier))
						e._sceneIdentifier = Guid.NewGuid().ToString();
					break;
				}
			}
		}
	}
}

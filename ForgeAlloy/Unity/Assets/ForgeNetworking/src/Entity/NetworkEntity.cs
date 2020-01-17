using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class NetworkEntity : MonoBehaviour, IUnityEntity
	{
		[SerializeField] private string _sceneIdentifier = null;
		public int Id { get; set; }
		public int PrefabId { get; set; }
		public GameObject OwnerGameObject => gameObject;
		public string SceneIdentifier
		{
			get => _sceneIdentifier;
			set { _sceneIdentifier = value; }
		}

		private void OnValidate()
		{
			var entities = GameObject.FindObjectsOfType<NetworkEntity>();
			List<string> _currentIds = entities.Select(e => e._sceneIdentifier).ToList();
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

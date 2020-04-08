using Forge.Factory;
using Puzzle.Network;
using UnityEngine;

namespace Puzzle.SemaphoreMarkers
{
	public class VisualSemaphore : MonoBehaviour
	{
		[SerializeField] private Transform _camTransform = null;
		[SerializeField] private GameObject _standardMarkerPrefab = null;
		[SerializeField] private int _standardMarkerId = 4;
		private IVisualSemaphoreMarker _standardMarker = null;
		private ComplexSampleNetwork _puzzle;

		public void Start()
		{
			_standardMarker = AbstractFactory.Get<IGameplayTypeFactory>().GetNew<IVisualSemaphoreMarker>();
			_standardMarker.Initialize(_standardMarkerPrefab);
			_puzzle = FindObjectOfType<ComplexSampleNetwork>();
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.F))
				SendStandardMarker(_standardMarker, _standardMarkerId);
		}

		private void SendStandardMarker(IVisualSemaphoreMarker marker, int netPrefabId)
		{
			if (marker == null) return;
			if (Physics.Raycast(_camTransform.position, _camTransform.forward, out var hit))
			{
				GameObject obj = marker.SetMarker(hit);
				_puzzle.SpawnRemotePrefab(netPrefabId, obj.transform.position,
					obj.transform.eulerAngles, obj.transform.localScale);
			}
		}
	}
}

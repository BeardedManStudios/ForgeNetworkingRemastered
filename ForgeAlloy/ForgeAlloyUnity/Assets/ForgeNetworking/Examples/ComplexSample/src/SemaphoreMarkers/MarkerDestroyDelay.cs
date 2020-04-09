using UnityEngine;

namespace Puzzle.SemaphoreMarkers
{
	public class MarkerDestroyDelay : MonoBehaviour
	{
		[SerializeField] private float _timeDelay = 5.0f;

		private void Start()
		{
			Destroy(gameObject, _timeDelay);
		}
	}
}

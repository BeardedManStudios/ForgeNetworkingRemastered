using UnityEngine;

namespace Puzzle.SemaphoreMarkers
{
	public interface IVisualSemaphoreMarker
	{
		void Initialize(GameObject prefab);
		GameObject SetMarker(RaycastHit hit);
	}
}

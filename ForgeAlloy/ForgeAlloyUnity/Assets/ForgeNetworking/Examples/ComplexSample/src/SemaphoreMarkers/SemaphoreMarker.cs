using UnityEngine;

namespace Puzzle.SemaphoreMarkers
{
	public class SemaphoreMarker : IVisualSemaphoreMarker
	{
		protected GameObject prefab = null;

		public void Initialize(GameObject prefab)
		{
			this.prefab = prefab;
		}

		public GameObject SetMarker(RaycastHit hit)
		{
			var obj = GameObject.Instantiate(prefab);
			obj.transform.position = hit.point;
			obj.transform.LookAt(hit.point + hit.normal);
			return obj;
		}
	}
}

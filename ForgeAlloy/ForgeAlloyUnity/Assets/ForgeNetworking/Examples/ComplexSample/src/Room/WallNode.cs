using UnityEngine;

namespace Puzzle.Room
{
	public class WallNode : MonoBehaviour, IWallNode
	{
		public GameObject GameObject => gameObject;

		[SerializeField, HideInInspector] private bool _top = true;
		[SerializeField, HideInInspector] private bool _bottom = true;
		[SerializeField, HideInInspector] private bool _left = true;
		[SerializeField, HideInInspector] private bool _right = true;
		[SerializeField, HideInInspector] private bool _front = true;
		[SerializeField, HideInInspector] private bool _back = true;

		[SerializeField] private GameObject _topWall = null;
		[SerializeField] private GameObject _bottomWall = null;
		[SerializeField] private GameObject _leftWall = null;
		[SerializeField] private GameObject _rightWall = null;
		[SerializeField] private GameObject _frontWall = null;
		[SerializeField] private GameObject _backWall = null;

		private void ResetWalls()
		{
			_top = true;
			_bottom = true;
			_left = true;
			_right = true;
			_front = true;
			_back = true;
		}

		private void UpdateActivity()
		{
			_topWall.SetActive(_top);
			_bottomWall.SetActive(_bottom);
			_leftWall.SetActive(_left);
			_rightWall.SetActive(_right);
			_frontWall.SetActive(_front);
			_backWall.SetActive(_back);
		}

		public void AutomaticWalls()
		{
			Transform parent = transform.parent;
			ResetWalls();
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform c = parent.GetChild(i);
				if (c == transform) continue;
				if (!c.gameObject.activeSelf) continue;
				WallNode wn = c.GetComponent<WallNode>();
				if (wn == null) continue;
				if (Vector3.Distance(c.position, transform.position) > 5.5f) continue;
				UpdateWallUsingOther(c);
			}
			UpdateActivity();
		}

		private void UpdateWallUsingOther(Transform other)
		{
			float len = Mathf.Max(Mathf.Max(transform.localScale.x, transform.localScale.y), transform.localScale.z);
			float min = len - 0.5f;
			float max = len + 0.5f;
			float iMin = -len - 0.5f;
			float iMax = -len + 0.5f;

			Vector3 delta = other.position - transform.position;

			if (delta.x < max && delta.x > min)
				_right = false;
			else if (delta.x < iMax && delta.x > iMin)
				_left = false;
			if (delta.y < max && delta.y > min)
				_top = false;
			else if (delta.y < iMax && delta.y > iMin)
				_bottom = false;
			if (delta.z < max && delta.z > min)
				_front = false;
			else if (delta.z < iMax && delta.z > iMin)
				_back = false;
		}
	}
}

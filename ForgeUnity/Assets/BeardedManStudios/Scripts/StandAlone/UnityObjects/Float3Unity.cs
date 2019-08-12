using UnityEngine;

namespace BeardedManStudios
{
	public partial struct Float3
	{
		public static implicit operator Float3(Vector3 v)
		{
			return new Float3(v.x, v.y, v.z);
		}

		public static implicit operator Vector3(Float3 f)
		{
			return new Vector3(f.x, f.y, f.z);
		}
	}
}

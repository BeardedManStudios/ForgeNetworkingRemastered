using UnityEngine;

namespace BeardedManStudios
{
	public partial struct Float2
	{
		public static implicit operator Float2(Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static implicit operator Vector2(Float2 f)
		{
			return new Vector2(f.x, f.y);
		}
	}
}

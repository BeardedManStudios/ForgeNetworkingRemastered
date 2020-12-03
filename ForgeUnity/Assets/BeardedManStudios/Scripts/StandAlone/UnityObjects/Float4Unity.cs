using UnityEngine;

namespace BeardedManStudios
{
	public partial struct Float4
	{
		public static implicit operator Float4(Vector4 v)
		{
			return new Float4(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Vector4(Float4 f)
		{
			return new Vector4(f.x, f.y, f.z, f.w);
		}

		public static implicit operator Float4(Quaternion q)
		{
			return new Float4(q.x, q.y, q.z, q.w);
		}

		public static implicit operator Quaternion(Float4 f)
		{
			return new Quaternion(f.x, f.y, f.z, f.w);
		}

		public static implicit operator Float4(Color c)
		{
			return new Float4(c.r, c.g, c.b, c.a);
		}

		public static implicit operator Color(Float4 f)
		{
			return new Color(f.x, f.y, f.z, f.w);
		}
	}
}

using System;

namespace BeardedManStudios
{
	public partial struct Float3
	{
		public float x, y, z;

		public Float3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static bool operator ==(Float3 a, Float3 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Float3 a, Float3 b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Float3))
				return false;

			Float3 other = (Float3)obj;
			return x == other.x && y == other.y && z == other.z;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", x, y, z);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static Float3 Lerp(Float3 a0, Float3 a1, float t)
		{
			return new Float3(
				BeardedMath.Lerp(a0.x, a1.x, t),
				BeardedMath.Lerp(a0.y, a1.y, t),
				BeardedMath.Lerp(a0.z, a1.z, t)
			);
		}

		public static float Distance(Float3 a, Float3 b)
		{
			return (float)Math.Sqrt((double)(
				Math.Pow(a.x - b.x, 2) +
				Math.Pow(a.y - b.y, 2) +
				Math.Pow(a.z - b.z, 2)
			));
		}
	}
}

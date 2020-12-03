using System;

namespace BeardedManStudios
{
	public partial struct Float2
	{
		public float x, y;

		public Float2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static bool operator ==(Float2 a, Float2 b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Float2 a, Float2 b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Float2))
				return false;

			Float2 other = (Float2)obj;
			return x == other.x && y == other.y;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", x, y);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static Float2 Lerp(Float2 a0, Float2 a1, float t)
		{
			return new Float2(
				BeardedMath.Lerp(a0.x, a1.x, t),
				BeardedMath.Lerp(a0.y, a1.y, t)
			);
		}

		public static float Distance(Float2 a, Float2 b)
		{
			return (float)Math.Sqrt((double)(
				Math.Pow(a.x - b.x, 2) +
				Math.Pow(a.y - b.y, 2)
			));
		}
	}
}

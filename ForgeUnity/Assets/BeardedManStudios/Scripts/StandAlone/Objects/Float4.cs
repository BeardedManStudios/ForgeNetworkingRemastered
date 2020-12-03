namespace BeardedManStudios
{
	public partial struct Float4
	{
		public float x, y, z, w;

		public Float4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static bool operator ==(Float4 a, Float4 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static bool operator !=(Float4 a, Float4 b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Float4))
				return false;

			Float4 other = (Float4)obj;
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2}, {3})", x, y, z, w);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

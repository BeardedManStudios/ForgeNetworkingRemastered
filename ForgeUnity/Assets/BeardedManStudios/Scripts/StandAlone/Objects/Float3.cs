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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

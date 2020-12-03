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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

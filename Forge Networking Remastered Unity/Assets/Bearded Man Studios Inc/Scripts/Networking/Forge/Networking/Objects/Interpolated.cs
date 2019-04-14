namespace BeardedManStudios.Forge.Networking
{
	public struct Interpolated<T> : IInterpolator<T>
	{
		public T current;
		public T target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public T Interpolate()
		{
			if (!Enabled) return target;

			current = BeardedMath.Lerp(current, target, LerpT);
			return current;
		}
	}
}

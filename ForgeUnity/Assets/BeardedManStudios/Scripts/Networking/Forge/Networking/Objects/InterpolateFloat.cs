namespace BeardedManStudios.Forge.Networking
{
	public struct InterpolateFloat : IInterpolator<float>
	{
		public float current;
		public float target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public float Interpolate()
		{
			if (!Enabled) return target;

			current = (float)BeardedMath.Lerp(current, target, LerpT);
			return current;
		}
	}
}

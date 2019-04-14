namespace BeardedManStudios.Forge.Networking
{
	public struct InterpolateDouble : IInterpolator<double>
	{
		public double current;
		public double target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public double Interpolate()
		{
			if (!Enabled) return target;

			current = (float)BeardedMath.Lerp(current, target, LerpT);
			return current;
		}
	}
}

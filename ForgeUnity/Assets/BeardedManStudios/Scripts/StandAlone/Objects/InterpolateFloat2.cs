using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios
{
	public struct InterpolateFloat2 : IInterpolator<Float2>
	{
		public Float2 current;
		public Float2 target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Float2 Interpolate()
		{
			if (!Enabled)
				return target;

			current = Float2.Lerp(current, target, LerpT);
			return current;
		}
	}
}

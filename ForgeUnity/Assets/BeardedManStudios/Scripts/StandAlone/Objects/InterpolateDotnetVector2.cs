using BeardedManStudios.Forge.Networking;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public struct InterpolateDotnetVector2 : IInterpolator<Numerics.Vector2>
	{
		public Numerics.Vector2 current;
		public Numerics.Vector2 target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Numerics.Vector2 Interpolate()
		{
			if (!Enabled) return target;

			current = Numerics.Vector2.Lerp(current, target, LerpT);
			return current;
		}
	}
}

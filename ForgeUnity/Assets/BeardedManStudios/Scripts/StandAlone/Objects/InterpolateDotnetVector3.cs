using BeardedManStudios.Forge.Networking;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public struct InterpolateDotnetVector3 : IInterpolator<Numerics.Vector3>
	{
		public Numerics.Vector3 current;
		public Numerics.Vector3 target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Numerics.Vector3 Interpolate()
		{
			if (!Enabled) return target;

			current = Numerics.Vector3.Lerp(current, target, LerpT);
			return current;
		}
	}
}

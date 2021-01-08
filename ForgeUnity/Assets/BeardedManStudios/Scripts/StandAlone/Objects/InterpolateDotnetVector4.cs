using BeardedManStudios.Forge.Networking;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public struct InterpolateDotnetVector4 : IInterpolator<Numerics.Vector4>
	{
		public Numerics.Vector4 current;
		public Numerics.Vector4 target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Numerics.Vector4 Interpolate()
		{
			if (!Enabled) return target;

			current = Numerics.Vector4.Lerp(current, target, LerpT);
			return current;
		}
	}
}

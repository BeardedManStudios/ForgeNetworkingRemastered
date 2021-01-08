using BeardedManStudios.Forge.Networking;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public struct InterpolateDotnetQuaternion : IInterpolator<Numerics.Quaternion>
	{
		public Numerics.Quaternion current;
		public Numerics.Quaternion target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Numerics.Quaternion Interpolate()
		{
			if (!Enabled) return target;

			current = Numerics.Quaternion.Slerp(current, target, LerpT);
			return current;
		}
	}
}

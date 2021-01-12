using UnityEngine;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public static class DotnetQuaternionUnity
	{
		public static Numerics.Quaternion ToDotnetQuaternion(this Quaternion v)
		{
			return new Numerics.Quaternion(v.x, v.y, v.z, v.w);
		}

		public static Quaternion ToUnityQuaternion(this Numerics.Quaternion f)
		{
			return new Quaternion(f.X, f.Y, f.Z, f.W);
		}
	}
}

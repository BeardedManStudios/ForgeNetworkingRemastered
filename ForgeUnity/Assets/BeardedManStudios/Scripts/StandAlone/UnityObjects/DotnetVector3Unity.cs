using UnityEngine;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public static class DotnetVector3Unity
	{
		public static Numerics.Vector3 ToDotnetVector(this Vector3 v)
		{
			return new Numerics.Vector3(v.x, v.y, v.z);
		}

		public static Vector3 ToUnityVector(this Numerics.Vector3 f)
		{
			return new Vector3(f.X, f.Y, f.Z);
		}
	}
}

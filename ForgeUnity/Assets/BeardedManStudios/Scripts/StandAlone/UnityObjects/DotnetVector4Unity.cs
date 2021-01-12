using UnityEngine;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public static class DotnetVector4Unity
	{
		public static Numerics.Vector4 ToDotnetVector(this Vector4 v)
		{
			return new Numerics.Vector4(v.x, v.y, v.z, v.w);
		}

		public static Vector4 ToUnityVector(this Numerics.Vector4 f)
		{
			return new Vector4(f.X, f.Y, f.Z, f.W);
		}

		public static Numerics.Vector4 ToDotnetVector(this Color v)
		{
			return new Numerics.Vector4(v.r, v.g, v.b, v.a);
		}

		public static Color ToUnityColor(this Numerics.Vector4 f)
		{
			return new Color(f.X, f.Y, f.Z, f.W);
		}
	}
}

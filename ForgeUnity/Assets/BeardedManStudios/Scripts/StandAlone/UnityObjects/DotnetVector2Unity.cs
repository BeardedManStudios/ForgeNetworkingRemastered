using UnityEngine;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
    public static class DotnetVector2Unity
	{
		public static Numerics.Vector2 ToDotnetVector(this Vector2 v)
		{
			return new Numerics.Vector2(v.x, v.y);
		}

		public static Vector2 ToUnityVector(this Numerics.Vector2 f)
		{
			return new Vector2(f.X, f.Y);
		}
	}
}

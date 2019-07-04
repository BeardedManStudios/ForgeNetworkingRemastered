using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public static class BMSUnityExtensions
	{
		public static bool UnityNear(this Vector2 target, Vector2 other, float threshold = 0.0012f)
		{
			return Vector2.Distance(target, other) <= threshold;
		}

		public static bool UnityNear(this Vector3 target, Vector3 other, float threshold = 0.0012f)
		{
			return Vector3.Distance(target, other) <= threshold;
		}

		public static bool UnityNear(this Vector4 target, Vector4 other, float threshold = 0.0012f)
		{
			return Vector4.Distance(target, other) <= threshold;
		}

		public static bool UnityNear(this Quaternion target, Quaternion other, float threshold = 0.0012f)
		{
			return Quaternion.Angle(target, other) <= threshold;
		}

		public static bool UnityNear<T>(this T target, T other, float threshold = 0.0012f)
		{
			return target.Near<T>(other, threshold);
		}
	}
}

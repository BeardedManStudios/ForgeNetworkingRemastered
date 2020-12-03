using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public static class BMSUnityExtensions
	{
        public static bool Near(this Vector2 target, Vector2 other, float distance = 0.0012f)
		{
            return Vector2.Distance(target, other) <= distance;
		}

        public static bool Near(this Vector3 target, Vector3 other, float distance = 0.0012f)
		{
            return Vector3.Distance(target, other) <= distance;
		}

        public static bool Near(this Vector4 target, Vector4 other, float distance = 0.0012f)
		{
            return Vector4.Distance(target, other) <= distance;
		}

        public static bool Near(this Quaternion target, Quaternion other, float distance = 0.0012f)
		{
            return Quaternion.Angle(target, other) <= distance;
        }
	}
}

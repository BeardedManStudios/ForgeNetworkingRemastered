/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

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
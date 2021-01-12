using System;
using Numerics = System.Numerics;

namespace BeardedManStudios
{
	public static class BMSExtensions
	{
		public static bool Between(this sbyte target, sbyte min, sbyte max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this byte target, byte min, byte max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this short target, short min, short max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this ushort target, ushort min, ushort max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this int target, int min, int max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this uint target, uint min, uint max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this long target, long min, long max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this ulong target, ulong min, ulong max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this float target, float min, float max)
		{
			return target >= min && target <= max;
		}

		public static bool Between(this double target, double min, double max)
		{
			return target >= min && target <= max;
		}

		public static bool Near(this float target, float other, float distance)
		{
			return target.Between(other - distance, other + distance);
		}

		public static bool Near(this Numerics.Vector2 target, Numerics.Vector2 other, float distance)
		{
			return Numerics.Vector2.Distance(target, other) <= distance;
		}

		public static bool Near(this Numerics.Vector3 target, Numerics.Vector3 other, float distance)
		{
			return Numerics.Vector3.Distance(target, other) <= distance;
		}

		public static bool Near(this Numerics.Vector4 target, Numerics.Vector4 other, float distance)
		{
			return Numerics.Vector4.Distance(target, other) <= distance;
		}

		public static bool Near(this Numerics.Quaternion target, Numerics.Quaternion other, float distance)
		{
			const double K_EPSILON = 0.000001;
			const double RADIANS_TO_DEGREES = 1.0 / (Math.PI / 180.0);

			float dotProduct = Numerics.Quaternion.Dot(target, other);

			double angle = dotProduct <= 1.0 - K_EPSILON ?
				Math.Acos(Math.Min(Math.Abs(dotProduct), 1.0F)) * 2.0 * RADIANS_TO_DEGREES :
				0.0;

			return angle <= distance;
		}

		public static bool Near<T>(this T target, T other, float distance)
		{
			return target.Equals(other);
		}
	}
}

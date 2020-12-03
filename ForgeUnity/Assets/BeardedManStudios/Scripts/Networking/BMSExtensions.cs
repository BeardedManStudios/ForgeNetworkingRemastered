﻿namespace BeardedManStudios
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

		public static bool Near(this Float2 target, Float2 other, float distance)
		{
			return Float2.Distance(target, other) <= distance;
		}

		public static bool Near(this Float3 target, Float3 other, float distance)
		{
			return Float3.Distance(target, other) <= distance;
		}

		public static bool Near(this Float4 target, Float4 other, float distance)
		{
			return Float4.Distance(target, other) <= distance;
		}

		public static bool Near<T>(this T target, T other, float distance)
		{
			return target.Equals(other);
		}
	}
}

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

		public static bool Near<T>(this T target, T other, float distance)
		{
			return target.Equals(other);
		}
	}
}
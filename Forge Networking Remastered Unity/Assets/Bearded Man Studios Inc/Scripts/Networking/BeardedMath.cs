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
	/// <summary>
	/// A class that is to be extended to support math models that are not currently available in any 
	/// currently available libraries
	/// </summary>
	public class BeardedMath
	{
		/// <summary>
		/// Precise method which guarantees v = v1 when t = 1, Mathf doesn't support doubles so this one will
		/// </summary>
		public static float Lerp(float a0, float a1, float t)
		{
			return (1 - t) * a0 + t * a1;
		}

		public static double Lerp(double a0, double a1, float t)
		{
			return (1 - t) * a0 + t * a1;
		}

		public static T Lerp<T>(T a0, T a1, float t)
		{
			return a0;
		}

		/// <summary>
		/// Used to determine if a specified object is a number type
		/// </summary>
		/// <param name="obj">The object to evaluate if it is a number</param>
		/// <returns>True if the supplied object is a numerical type</returns>
		public static bool IsNumber(object obj)
		{
			return obj is sbyte
				|| obj is byte
				|| obj is short
				|| obj is ushort
				|| obj is int
				|| obj is uint
				|| obj is long
				|| obj is ulong
				|| obj is float
				|| obj is double
				|| obj is decimal;
		}
	}
}
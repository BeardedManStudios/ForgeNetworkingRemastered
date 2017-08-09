using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace UnitTests
{
	public class BaseTest
	{
		protected delegate bool BooleanEvent();

		/// <summary>
		/// A simple method to wait for a given amount of time (default 10 seconds)
		/// </summary>
		/// <param name="condition">The condition that must be true before this function exits (before time runs out)</param>
		/// <param name="sleep">How long should be slept between each iteration of the loop</param>
		/// <param name="iterations">The amount of iterations of the loop</param>
		/// <returns></returns>
		protected static void WaitFor(Func<bool> condition, int iterations = 5000)
		{
			int counter = 0;
			do
			{
				Thread.Sleep(1);
			} while (++counter < iterations && !condition());

			Assert.IsTrue(condition());
		}
	}
}
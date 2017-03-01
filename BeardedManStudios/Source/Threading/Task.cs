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

using System;
using System.Threading;

namespace BeardedManStudios.Threading
{
	public static class ThreadManagement
	{
		public static int MainThreadId { get; private set; }

		public static int GetCurrentThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		public static void Initialize() { MainThreadId = GetCurrentThreadId(); }

		public static bool IsMainThread
		{
			get { return GetCurrentThreadId() == MainThreadId; }
		}
	}

	/// <summary>
	/// A class for calling methods on a separate thread
	/// </summary>
	public static class Task
	{
		/// <summary>
		/// Sets the method that is to be executed on the separate thread
		/// </summary>
		/// <param name="expression">The method that is to be called on the newly created thread</param>
		private static void QueueExpression(WaitCallback expression)
		{
			ThreadPool.QueueUserWorkItem(expression);
		}

		/// <summary>
		/// Used to run a method / expression on a separate thread
		/// </summary>
		/// <param name="expression">The method to be run on the separate thread</param>
		/// <param name="delayOrSleep">The amount of time to wait before running the expression on the newly created thread</param>
		/// <returns></returns>
		public static void Queue(Action expression, int delayOrSleep = 0)
		{
			// Wrap the expression in a method so that we can apply the delayOrSleep before and remove the task after it finishes
			WaitCallback inline = (state) =>
			{
				// Apply the specified delay
				if (delayOrSleep > 0)
					Thread.Sleep(delayOrSleep);

				// Call the requested method
				expression();
			};

			// Set the method to be called on the separate thread to be the inline method we have just created
			QueueExpression(inline);
		}

#if WINDOWS_UWP
		public async static void Sleep(int milliseconds)
		{
			await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(milliseconds));
		}
#else
		public static void Sleep(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}
#endif
	}
}
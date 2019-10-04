﻿using System;
using System.Threading;
using BeardedManStudios.Forge.Logging;

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
		/// Event that is signaled when all task threads have completed their execution
		/// </summary>
		public static ManualResetEvent threadsJoinedEvent = new ManualResetEvent(true);

		private static int numRunningTasks = 0;

		/// <summary>
		/// Sets the method that is to be executed on the separate thread
		/// </summary>
		/// <param name="expression">The method that is to be called on the newly created thread</param>
		private static void QueueExpression(WaitCallback expression)
		{
			Interlocked.Increment(ref numRunningTasks);
			threadsJoinedEvent.Reset();
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
				if (Interlocked.Decrement(ref numRunningTasks) == 0)
				{
					threadsJoinedEvent.Set();
				}
			};

			// Set the method to be called on the separate thread to be the inline method we have just created
			QueueExpression(inline);
		}

		public static void Sleep(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}

		/// <summary>
		/// Block execution until all enqueued tasks have completed
		/// </summary>
		public static void WaitAll()
		{
			while (!threadsJoinedEvent.WaitOne(1000, false))
			{
				BMSLog.LogFormat("Task: WaitAll waited for 1s");
			}
		}
	}
}

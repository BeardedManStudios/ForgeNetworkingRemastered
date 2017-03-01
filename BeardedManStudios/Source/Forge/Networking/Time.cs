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

using System.Diagnostics;

namespace BeardedManStudios.Forge.Networking
{
	public class TimeManager
	{
		public Stopwatch timer { get; private set; }

		private ulong timeOffset = 0;

		public long Milliseconds { get { return timer.ElapsedMilliseconds; } }

		public ulong Timestep { get { return (ulong)Milliseconds + timeOffset; } }

		public TimeManager()
		{
			timer = new Stopwatch();
			timer.Start();
		}

		public void SetStartTime(ulong timeStep)
		{
			timeOffset = timeStep - (ulong)timer.ElapsedMilliseconds;
		}
	}
}
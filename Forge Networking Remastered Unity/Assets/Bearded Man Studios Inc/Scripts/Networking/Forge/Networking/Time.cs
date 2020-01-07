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

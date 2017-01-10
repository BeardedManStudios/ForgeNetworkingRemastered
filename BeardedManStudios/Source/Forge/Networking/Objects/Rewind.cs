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
|  copyrighted by Bearded Man Studios, Inc. (2012-2016) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

using System.Collections.Generic;
using System.Linq;

namespace BeardedManStudios.Forge.Networking
{
	public class Rewind<T>
	{
		public TimeManager Time { get; set; }
		public ulong rewindTime = 0;
		private Dictionary<ulong, T> rewinds = null;

		public void Register(T value, ulong timestep = 0)
		{
			if (timestep == 0)
				timestep = Time.Timestep;

			if (rewinds.Count > 0)
			{
				var keys = rewinds.Keys;
				foreach (ulong key in keys)
				{
					if (key < timestep - rewindTime)
						rewinds.Remove(key);
				}
			}

			rewinds.Add(timestep, value);
		}

		/// <summary>
		/// Used to get the value of a variable based on a given time step
		/// </summary>
		/// <param name="timestep">The time step that a value should be pulled for</param>
		/// <returns>The value of the tracked variable at the given time stamp or default() if no history is available</returns>
		public T Get(ulong timestep)
		{
			if (rewinds.Count == 0)
				return default(T);

			if (rewinds.ContainsKey(timestep))
				return rewinds[timestep];

			var keys = rewinds.Keys;
			ulong key = 0;

			foreach (ulong k in keys)
			{
				if (timestep < k)
				{
					if (timestep - key > k - timestep) key = k;
					break;
				}

				key = k;
			}

			return rewinds[key];
		}

		/// <summary>
		/// Used to get the value of a variable based on a given time step but also
		/// will out the lower and upper if found to allow for getting median
		/// </summary>
		/// <param name="timestep">The time step that a value should be pulled for</param>
		/// <param name="lower">The lover value relative to the timestep</param>
		/// <param name="upper">The upper value relative to the timestep</param>
		/// <returns>The value of the tracked variable at the given time stamp or default() if no history is available</returns>
		public T Get(ulong timestep, out T lower, out T upper)
		{
			lower = default(T);
			upper = lower;

			if (rewinds.Count == 0)
				return lower;

			if (rewinds.ContainsKey(timestep))
				return rewinds[timestep];

			var keys = rewinds.Keys;
			ulong key = 0, lowerKey = 0, upperKey = 0;

			if (keys.Last() < timestep)
			{
				// TODO:  The supplied time does not exist, throw exception?
				return default(T);
			}

			foreach (ulong k in keys)
			{
				if (timestep < k)
				{
					lowerKey = key;
					upperKey = k;
					if (timestep - key > k - timestep) key = k;
					break;
				}

				key = k;
			}

			lower = rewinds[lowerKey];
			upper = rewinds[upperKey];
			return rewinds[key];
		}

		/// <summary>
		/// Gets a range of values leading up to a given time step
		/// </summary>
		/// <param name="timestep">The time step that a value should be pulled for</param>
		/// <param name="count">The number of elements to get before the time step</param>
		/// <returns>The list of found elements leading up to the timestep (including time step value if found</returns>
		public List<T> GetRange(ulong timestep, int count)
		{
			List<T> found = new List<T>();

			if (rewinds.Count == 0)
				return found;

			var keys = rewinds.Keys.ToArray().Reverse();
			ulong lastKey = 0;

			if (keys.First() < timestep)
			{
				// TODO:  The supplied time does not exist, throw exception?
				return found;
			}

			foreach (ulong k in keys)
			{
				if (timestep <= k)
				{
					if (timestep != k && lastKey != 0)
					{
						found.Add(rewinds[lastKey]);
						lastKey = 0;
					}

					found.Insert(0, rewinds[k]);

					if (found.Count == count)
						break;
				}
				else
					lastKey = k;
			}

			return found;
		}
	}
}
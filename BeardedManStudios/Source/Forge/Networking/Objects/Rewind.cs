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

using System.Collections.Generic;
using System.Linq;

namespace BeardedManStudios.Forge.Networking
{
    public class Rewind<T>
    {
        public ulong RewindTime { get; set; }
        private Dictionary<ulong, T> rewinds = null;
        private List<ulong> keys = new List<ulong>();

        public Rewind(ulong length)
        {
            RewindTime = length;
            rewinds = new Dictionary<ulong, T>();
        }

        public void Register(T value, ulong timestep = 0)
        {
            lock (rewinds)
            {
                if (keys.Contains(timestep))
                {
                    rewinds[timestep] = value;
                    return;
                }

                if (rewinds.Count > 0)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i] < timestep - RewindTime && RewindTime < timestep)
                        {
                            rewinds.Remove(keys[i]);
                            keys.RemoveAt(i--);
                        }
                        else
                            break;
                    }
                }

                rewinds.Add(timestep, value);
                keys.Add(timestep);
            }
        }

        /// <summary>
        /// Used to get the value of a variable based on a given time step
        /// </summary>
        /// <param name="timestep">The time step that a value should be pulled for</param>
        /// <returns>The value of the tracked variable at the given time stamp or default() if no history is available</returns>
        public T Get(ulong timestep)
        {
            lock (rewinds)
            {
                if (rewinds.Count == 0)
                    return default(T);

                T result;
                if (rewinds.TryGetValue(timestep, out result))
                    return result;

                ulong key = 0;
                foreach (ulong k in keys)
                {
                    if (timestep < k)
                    {
                        if (timestep - key > k - timestep)
                            key = k;

                        break;
                    }

                    key = k;
                }

                return rewinds[key];
            }
        }

        /// <summary>
        /// Used to get the value of a variable based on a given time step but also
        /// will out the lower and upper if found to allow for getting median
        /// </summary>
        /// <param name="timestep">The time step that a value should be pulled for</param>
        /// <param name="lower">The lower value relative to the timestep</param>
        /// <param name="upper">The upper value relative to the timestep</param>
        /// <returns>The value of the tracked variable at the given time stamp or default() if no history is available</returns>
        public T Get(ulong timestep, out T lower, out T upper)
        {
            lock (rewinds)
            {
                lower = default(T);
                upper = lower;

                if (rewinds.Count == 0)
                    return lower;

                if (rewinds.ContainsKey(timestep))
                    return rewinds[timestep];

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
                        if (timestep - key > k - timestep)
                            key = k;

                        break;
                    }

                    key = k;
                }

                lower = rewinds[lowerKey];
                upper = rewinds[upperKey];
                return rewinds[key];
            }
        }

        /// <summary>
        /// Gets a range of values leading up to a given time step
        /// </summary>
        /// <param name="timestep">The time step that a value should be pulled for</param>
        /// <param name="count">The number of elements to get before the time step</param>
        /// <returns>The list of found elements leading up to the timestep (including time step value if found</returns>
        public List<T> GetRange(ulong timestep, int count)
        {
            lock (rewinds)
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

        /// <summary>
        /// Gets a range of values between the two timesteps (inclusive)
        /// </summary>
        /// <param name="timestepMin">The minimum inclusive timestep that a value should be pulled for</param>
        /// <param name="timestepMax">The maximum inclusive timestep that a value should be pulled for</param>
        /// <returns>The list of found elements between the two timesteps inclusively </returns>
        public List<T> GetRange(ulong timestepMin, ulong timestepMax)
        {
            lock (rewinds)
            {
                List<T> found = new List<T>();

                if (rewinds.Count == 0)
                    return found;

                var keys = rewinds.Keys.ToArray().Reverse();

                if (keys.First() < timestepMin)
                {
                    // TODO:  The supplied time does not exist, throw exception?
                    return found;
                }

                foreach (ulong k in keys)
                {
                    if (timestepMin <= k && timestepMax >= k)
                        found.Insert(0, rewinds[k]);
                }

                return found;
            }
        }
    }
}
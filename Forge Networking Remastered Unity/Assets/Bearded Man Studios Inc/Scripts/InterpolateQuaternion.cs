/*-----------------------------+------------------------------\
|                                                             |
|                        !!!NOTICE!!!                         |
|                                                             |
|  These libraries are under heavy development so they are    |
|  subject to make many changes as development continues.     |
|  For this reason, the libraries may not be well commented.  |
|  THANK YOU for supporting forge with all your feedback      |
|  suggestions, bug reports and comments!                     |
|                                                             |
|                               - The Forge Team              |
|                                 Bearded Man Studios, Inc.   |
|                                                             |
|  This source code, project files, and associated files are  |
|  copyrighted by Bearded Man Studios, Inc. (2012-2015) and   |
|  may not be redistributed without written permission.       |
|                                                             |
\------------------------------+-----------------------------*/



using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public struct InterpolateQuaternion : IInterpolator<Quaternion>
	{
		public Quaternion current;
		public Quaternion target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Quaternion Interpolate()
		{
			if (!Enabled) return target;

			current = Quaternion.Slerp(current, target, LerpT);
			return current;
		}
	}
}
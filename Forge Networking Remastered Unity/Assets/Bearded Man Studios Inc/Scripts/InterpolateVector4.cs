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
	public struct InterpolateVector4 : IInterpolator<Vector4>
	{
		public Vector4 current;
		public Vector4 target;
		public float LerpT { get; set; }
		public bool Enabled { get; set; }
		public ulong Timestep { get; set; }

		public Vector4 Interpolate()
		{
			if (!Enabled) return target;

			current = Vector3.Lerp(current, target, LerpT);
			return current;
		}
	}
}
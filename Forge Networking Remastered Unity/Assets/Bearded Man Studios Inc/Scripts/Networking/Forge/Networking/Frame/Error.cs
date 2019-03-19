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

using System.Text;

namespace BeardedManStudios.Forge.Networking.Frame
{
	public class Error : FrameStream
	{
		/// <summary>
		/// Connection close frame is denoted by x0
		/// </summary>
		public const byte CONTROL_BYTE = 0;

		public override byte ControlByte { get { return CONTROL_BYTE; } }

		public Error() : base() { }
		public Error(ulong timestep, bool useMask, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, receivers, groupId, isStream) { }
		public Error(ulong timestep, bool useMask, byte[] payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Error(ulong timestep, bool useMask, BMSByte payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Error(byte[] frame, int payloadStart, int groupId, NetworkingPlayer sender, byte receivers) : base(frame, payloadStart, groupId, sender, receivers) { }

		public static Error CreateErrorMessage(ulong timestep, string message, bool useMask, int groupId, bool isStream)
		{
			return new Error(timestep, useMask, Encoding.UTF8.GetBytes(message), Receivers.Target, groupId, isStream);
		}

		public override object Clone()
		{
			return BaseClone(new Binary());
		}
	}
}
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
	public class Text : FrameStream
	{
		/// <summary>
		/// Connection close frame is denoted by %x1
		/// </summary>
		public const byte CONTROL_BYTE = 129;

		public override byte ControlByte { get { return CONTROL_BYTE; } }

		public Text() : base() { }
		public Text(ulong timestep, bool useMask, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, receivers, groupId, isStream) { }
		public Text(ulong timestep, bool useMask, byte[] payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Text(ulong timestep, bool useMask, BMSByte payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Text(byte[] frame, int payloadStart, int groupId, NetworkingPlayer sender, byte receivers) : base(frame, payloadStart, groupId, sender, receivers) { }

		/// <summary>
		/// Create a Text frame from a string rather than raw byte data
		/// </summary>
		/// <param name="timestep">The timestep for the frame</param>
		/// <param name="message">The string message that this Text frame represents</param>
		/// <param name="useMask">If a mask should be applied to this data</param>
		/// <param name="receivers">The receivers on the network for this Text frame</param>
		/// <param name="groupId">The unique message group id for this Text frame</param>
		/// <param name="isStream">If this data is being sent as a stream or in packets</param>
		/// <returns></returns>
		public static Text CreateFromString(ulong timestep, string message, bool useMask, Receivers receivers, int groupId, bool isStream)
		{
			return new Text(timestep, useMask, Encoding.UTF8.GetBytes(message), receivers, groupId, isStream);
		}

		/// <summary>
		/// Converts the data bytes of this frame to a string
		/// </summary>
		/// <returns>The string representation of the data bytes for this frame</returns>
		public override string ToString()
		{
			string stringData = Encoding.UTF8.GetString(StreamData.CompressBytes());

			if (IsReliable)
				return stringData.Remove(stringData.Length - sizeof(ulong));

			return stringData;
		}

		public override object Clone()
		{
			return BaseClone(new Binary());
		}
	}
}
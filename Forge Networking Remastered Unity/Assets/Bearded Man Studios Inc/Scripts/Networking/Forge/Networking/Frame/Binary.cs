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


namespace BeardedManStudios.Forge.Networking.Frame
{
	public class Binary : FrameStream
	{
		/// <summary>
		/// Connection close frame is denoted by %x2
		/// </summary>
		public const byte CONTROL_BYTE = 130;

		public override byte ControlByte { get { return CONTROL_BYTE; } }

		public Binary() : base() { }
		public Binary(ulong timestep, bool useMask, Receivers receivers, int groupId, bool isStream, byte routerId = 0) : base(timestep, useMask, receivers, groupId, isStream, routerId) { }
		public Binary(ulong timestep, bool useMask, byte[] payload, Receivers receivers, int groupId, bool isStream, byte routerId = 0) : base(timestep, useMask, payload, receivers, groupId, isStream, routerId) { }
		public Binary(ulong timestep, bool useMask, BMSByte payload, Receivers receivers, int groupId, bool isStream, byte routerId = 0) : base(timestep, useMask, payload, receivers, groupId, isStream, routerId) { }
		public Binary(byte[] frame, int payloadStart, int groupId, NetworkingPlayer sender, byte receivers) : base(frame, payloadStart, groupId, sender, receivers) { }

		/// <summary>
		/// Take an existing byte[] frame and map it to this data type
		/// </summary>
		/// <param name="frame">The existing frame data</param>
		/// <param name="payloadStart">The index that the payload starts at in the frame byte[]</param>
		protected override void ReadFrame(byte[] frame, int payloadStart, byte receivers = 255)
		{
			// Read the router byte
			RouterId = frame[payloadStart];

			// Move the start of the payload since we have read the router byte
			payloadStart++;

			base.ReadFrame(frame, payloadStart, receivers);
		}

		public override object Clone()
		{
			return BaseClone(new Binary());
		}
	}
}
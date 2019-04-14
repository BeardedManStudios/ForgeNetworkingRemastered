namespace BeardedManStudios.Forge.Networking.Frame
{
	public class Ping : FrameStream
	{
		/// <summary>
		/// Connection close frame is denoted by %x9
		/// </summary>
		public const byte CONTROL_BYTE = 137;

		public override byte ControlByte { get { return CONTROL_BYTE; } }

		public Ping() : base() { }
		public Ping(ulong timestep, bool useMask, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, receivers, groupId, isStream) { }
		public Ping(ulong timestep, bool useMask, byte[] payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Ping(ulong timestep, bool useMask, BMSByte payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Ping(byte[] frame, int payloadStart, int groupId, NetworkingPlayer sender, byte receivers) : base(frame, payloadStart, groupId, sender, receivers) { }

		public override object Clone()
		{
			return BaseClone(new Binary());
		}
	}
}

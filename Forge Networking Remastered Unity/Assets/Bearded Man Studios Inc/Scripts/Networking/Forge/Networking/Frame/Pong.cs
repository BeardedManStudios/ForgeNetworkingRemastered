namespace BeardedManStudios.Forge.Networking.Frame
{
	public class Pong : FrameStream
	{
		/// <summary>
		/// Connection close frame is denoted by %xA
		/// </summary>
		public const byte CONTROL_BYTE = 138;

		public override byte ControlByte { get { return CONTROL_BYTE; } }

		public Pong() : base() { }
		public Pong(ulong timestep, bool useMask, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, receivers, groupId, isStream) { }
		public Pong(ulong timestep, bool useMask, byte[] payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Pong(ulong timestep, bool useMask, BMSByte payload, Receivers receivers, int groupId, bool isStream) : base(timestep, useMask, payload, receivers, groupId, isStream) { }
		public Pong(byte[] frame, int payloadStart, int groupId, NetworkingPlayer sender, byte receivers) : base(frame, payloadStart, groupId, sender, receivers) { }

		public override object Clone()
		{
			return BaseClone(new Binary());
		}
	}
}

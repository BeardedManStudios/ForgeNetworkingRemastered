namespace BeardedManStudios.Forge.Networking.SQP
{
	public struct QueryHeader : ISQPMessage
	{
		public byte Type { get; internal set; }
		public uint ChallengeId;

		public void Serialize(ref BMSByte buffer)
		{
			buffer = ObjectMapper.BMSByte(Type, ChallengeId);
		}

		public void Deserialize(ref BMSByte buffer)
		{
			Type = buffer.GetBasicType<byte>();
			ChallengeId = (uint) System.Net.IPAddress.NetworkToHostOrder((int) buffer.GetBasicType<uint>());
		}
	}
}

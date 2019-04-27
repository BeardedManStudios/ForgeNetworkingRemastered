namespace BeardedManStudios.Forge.Networking.SQP
{
	public struct QueryHeader : ISQPMessage
	{
		public byte Type { get; internal set; }
		public uint ChallengeId;

		public void Serialize(ref BMSByte buffer)
		{
			ObjectMapper.Instance.MapBytes(buffer, Type, ChallengeId);
		}

		public void Deserialize(BMSByte buffer)
		{
			Type = buffer.GetBasicType<byte>();
			ChallengeId = (uint) System.Net.IPAddress.NetworkToHostOrder((int) buffer.GetBasicType<uint>());
		}
	}
}

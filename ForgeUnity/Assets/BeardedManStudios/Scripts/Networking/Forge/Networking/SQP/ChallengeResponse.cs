namespace BeardedManStudios.Forge.Networking.SQP
{
	public struct ChallengeResponse
	{
		public QueryHeader Header;

		public void Serialize(ref BMSByte buffer)
		{
			Header.Type = (byte)MessageType.ChallengeResponse;
			Header.Serialize(ref buffer);
		}

		public void Deserialize(ref BMSByte buffer)
		{
			Header.Deserialize(buffer);
		}
	}
}

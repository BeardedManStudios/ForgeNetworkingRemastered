namespace BeardedManStudios.Forge.Networking.SQP
{
	public struct ChallengeRequest : ISQPMessage
	{
		public QueryHeader Header;

		public void Serialize(ref BMSByte buffer)
		{
			Header.Type = (byte)MessageType.ChallengeRequest;
			Header.Serialize(ref buffer);
		}

		public void Deserialize(BMSByte buffer)
		{
			Header.Deserialize(buffer);
		}
	}
}

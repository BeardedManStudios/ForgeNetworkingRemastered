namespace BeardedManStudios.Forge.Networking.SQP
{
	public class QueryRequest : ISQPMessage
	{
		public QueryHeader Header;
		public ushort Version;

		/// <summary>
		/// The <see cref="ChunkType"/> that is requested
		/// </summary>
		public byte RequestedChunks;

		public void Serialize(ref BMSByte buffer)
		{
			Header.Type = (byte)MessageType.QueryRequest;

			Header.Serialize(ref buffer);
			ObjectMapper.Instance.MapBytes(buffer, (ushort) System.Net.IPAddress.HostToNetworkOrder((short) Version), RequestedChunks);
		}

		public void Deserialize(BMSByte buffer)
		{
			Header.Deserialize(buffer);
			Version = (ushort) System.Net.IPAddress.NetworkToHostOrder((short) buffer.GetBasicType<ushort>());
			RequestedChunks = buffer.GetBasicType<byte>();
		}
	}
}

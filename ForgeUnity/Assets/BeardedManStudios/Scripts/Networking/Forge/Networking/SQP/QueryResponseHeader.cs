namespace BeardedManStudios.Forge.Networking.SQP
{
	public struct QueryResponseHeader : ISQPMessage
	{
		public QueryHeader Header;
		public ushort Version;
		public ushort Length;

		public void Serialize(ref BMSByte buffer)
		{
			Header.Type = (byte)MessageType.QueryResponse;
			Header.Serialize(ref buffer);

			var v = (ushort) System.Net.IPAddress.HostToNetworkOrder((short) Version);
			var l = (ushort) System.Net.IPAddress.HostToNetworkOrder((short) Length);

			ObjectMapper.Instance.MapBytes(buffer, v, l);
		}

		public void Deserialize(BMSByte buffer)
		{
			Header.Deserialize(buffer);
			Version = (ushort) System.Net.IPAddress.NetworkToHostOrder((short) buffer.GetBasicType<ushort>());
			Length = (ushort) System.Net.IPAddress.NetworkToHostOrder((short) buffer.GetBasicType<ushort>());
		}
	}
}

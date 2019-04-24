namespace BeardedManStudios.Forge.Networking.SQP
{
	public class ServerInfo
	{
		public QueryResponseHeader QueryHeader;
		public uint ChunkLen;
		public Data ServerInfoData;

		public ServerInfo()
		{
			ServerInfoData = new Data();
		}



		public void Serialize(ref BMSByte buffer)
		{
			QueryHeader.Serialize(ref buffer);
		}

		public void Deserialize(ref BMSByte buffer)
		{
			QueryHeader.Deserialize(ref buffer);
		}

		public class Data
		{
			public ushort CurrentPlayers;
			public ushort MaxPlayers;
			public string ServerName = "";
			public string ServerType = "";
			public ushort Port;

			public void Serialize(ref BMSByte buffer)
			{
				var currPlayers = (ushort) System.Net.IPAddress.HostToNetworkOrder((short) CurrentPlayers);
				var maxPlayers = (ushort) System.Net.IPAddress.HostToNetworkOrder((short) MaxPlayers);
				var port = (ushort) System.Net.IPAddress.HostToNetworkOrder((short) Port);

				ObjectMapper.Instance.MapBytes(buffer, currPlayers, maxPlayers, ServerName, ServerType, port);
			}

			public void Desrialize(ref BMSByte buffer)
			{
				CurrentPlayers = (ushort) System.Net.IPAddress.NetworkToHostOrder((short) buffer.GetBasicType<ushort>());
				MaxPlayers = (ushort) System.Net.IPAddress.NetworkToHostOrder((short) buffer.GetBasicType<ushort>());

				ServerName = buffer.GetBasicType<string>();
				ServerType = buffer.GetBasicType<string>();
				Port = (ushort) System.Net.IPAddress.NetworkToHostOrder((short) buffer.GetBasicType<ushort>());
			}
		}
	}
}

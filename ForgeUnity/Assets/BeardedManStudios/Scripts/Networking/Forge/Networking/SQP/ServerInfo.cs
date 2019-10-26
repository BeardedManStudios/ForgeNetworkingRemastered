using System;
using System.Text;

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
			ServerInfoData.Serialize(buffer);
		}

		public void Deserialize(BMSByte buffer)
		{
			QueryHeader.Deserialize(buffer);
			ServerInfoData.Desrialize(buffer);
		}

		public class Data
		{
			public ushort CurrentPlayers;
			public ushort MaxPlayers;
			public string ServerName = "";
			public string ServerType = "";
			public ushort Port;

			public void Serialize(BMSByte buffer)
			{
				ObjectMapper.Instance.MapBytes(buffer, CurrentPlayers, MaxPlayers, ServerName, ServerType, Port);
			}

			public void Desrialize(BMSByte buffer)
			{
				CurrentPlayers = buffer.GetBasicType<ushort>();
				MaxPlayers = buffer.GetBasicType<ushort>();
				ServerName = buffer.GetBasicType<string>();
				ServerType = buffer.GetBasicType<string>();
				Port = buffer.GetBasicType<ushort>();
			}
		}
	}
}

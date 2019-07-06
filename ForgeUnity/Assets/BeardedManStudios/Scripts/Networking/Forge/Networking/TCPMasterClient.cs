using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.SimpleJSON;

namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// The purpose of this master client is for the master server to register
	/// </summary>
	public class TCPMasterClient : TCPClient
	{
		private JSONNode registerServerData;

		public static JSONNode CreateMasterServerRegisterData(NetWorker server, string id, string serverName,
			string type, string mode, string comment, bool useElo, int eloRequired)
		{
			var sendData = JSONNode.Parse("{}");
			var registerData = new JSONClass();
			registerData.Add("id", id);
			registerData.Add("name", serverName);
			registerData.Add("port", new JSONData(server.Port));
			registerData.Add("playerCount", new JSONData(server.Players.Count));
			registerData.Add("maxPlayers", new JSONData(server.MaxConnections));
			registerData.Add("comment", comment);
			registerData.Add("type", type);
			registerData.Add("mode", mode);
			registerData.Add("protocol", server is UDPServer ? "udp" : "tcp");
			registerData.Add("elo", new JSONData(eloRequired));
			registerData.Add("useElo", new JSONData(useElo));
			sendData.Add("register", registerData);
			return sendData;
		}

		protected override void Initialize(string host, ushort port, bool pendCreates = false)
		{
			base.Initialize(host, port, pendCreates);
			base.InitializeTCPClient(host, port);
		}

		public void RegisterOnMasterServer(string hostAddress, ushort port, JSONNode masterServerData)
		{
			registerServerData = masterServerData;
			serverAccepted += SendRegisterRequestToMasterServer;
			Connect(hostAddress, port);
		}

		private void SendRegisterRequestToMasterServer(NetWorker sender)
		{
			try
			{
				var registerRequest = Text.CreateFromString(Time.Timestep, registerServerData.ToString(), true,
				Receivers.Server, MessageGroupIds.MASTER_SERVER_REGISTER, true);
				Send(registerRequest);
			}
			catch
			{
				// If anything fails, then this client needs to be disconnected
				Disconnect(true);
			}
		}
	}
}

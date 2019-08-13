using System.Net;

namespace BeardedManStudios.Forge.Networking.SQP
{
	public class Query
	{
		public IPEndPoint Server;
		public bool ValidResult;
		public ClientState State;
		public uint ChallengeId;
		public long RTT;
		public long StartTime;
		public ServerInfo ServerInfo;

		public Query()
		{
			ServerInfo = new ServerInfo();
			State = ClientState.Idle;
			Server = null;
		}

		public void Init(IPEndPoint server)
		{
			Server = server;
			ValidResult = false;
		}
	}
}

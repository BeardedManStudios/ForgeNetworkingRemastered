using BeardedManStudios.Forge.Networking;

namespace MasterServer
{
	public struct Host
	{
		public string Name;
		public string Address;
		public ushort Port;
		public int PlayerCount;
		public int MaxPlayers;
		public string Comment;
		public string Id;
		public string Type;
		public string Mode;
		public string Protocol;
		public int Elo;
		public bool UseElo;
		public NetworkingPlayer Player;
	}
}
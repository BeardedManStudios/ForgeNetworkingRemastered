#if STEAMWORKS || FACEPUNCH_STEAMWORKS
using Steamworks;

namespace BeardedManStudios.Forge.Networking
{
	public class SteamNetworkingPlayer : NetworkingPlayer
	{
		public UDPPacketManager PacketManager { get; private set; }

#if STEAMWORKS
		public SteamNetworkingPlayer(uint networkId, CSteamID steamId, bool isHost, NetWorker networker) : base(networkId, "", isHost, null, networker)
#else
		public SteamNetworkingPlayer(uint networkId, SteamId steamId, bool isHost, NetWorker networker) : base(networkId, "", isHost, null, networker)
#endif
		{
			PacketManager = new UDPPacketManager();
			SteamID = steamId;
		}
	}
}
#endif

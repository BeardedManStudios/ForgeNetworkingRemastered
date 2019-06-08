#if FACEPUNCH_STEAMWORKS
using Steamworks;

namespace BeardedManStudios.Forge.Networking
{
	public class FacepunchNetworkingPlayer : NetworkingPlayer
	{
		public UDPPacketManager PacketManager { get; private set; }

		public FacepunchNetworkingPlayer(uint networkId, SteamId steamId, bool isHost, NetWorker networker) : base(networkId, "", isHost, null, networker)
		{
			PacketManager = new UDPPacketManager();
			SteamID = steamId;
		}
	}
}
#endif

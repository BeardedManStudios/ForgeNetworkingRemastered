#if STEAMWORKS
using Steamworks;

namespace BeardedManStudios.Forge.Networking
{
    public class SteamNetworkingPlayer : NetworkingPlayer
    {
        public UDPPacketManager PacketManager { get; private set; }

        public SteamNetworkingPlayer(uint networkId, CSteamID steamId, bool isHost, NetWorker networker) : base(networkId, "", isHost, null, networker)
        {
            PacketManager = new UDPPacketManager();
            SteamID = steamId;
        }
    }
}
#endif

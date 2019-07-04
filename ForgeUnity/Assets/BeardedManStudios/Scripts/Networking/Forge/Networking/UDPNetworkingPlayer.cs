namespace BeardedManStudios.Forge.Networking
{
	public class UDPNetworkingPlayer : NetworkingPlayer
	{
		public UDPPacketManager PacketManager { get; private set; }

		public UDPNetworkingPlayer(uint networkId, string ip, bool isHost, object socketEndpoint, NetWorker networker) : base(networkId, ip, isHost, socketEndpoint, networker)
		{
			PacketManager = new UDPPacketManager();
		}
	}
}

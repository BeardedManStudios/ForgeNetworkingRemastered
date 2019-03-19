/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

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
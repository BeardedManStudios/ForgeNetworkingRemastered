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

using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking
{
	public class UDPPacketManager
	{
		public Dictionary<int, UDPPacketGroup> packetGroups = new Dictionary<int, UDPPacketGroup>();

		/// <summary>
		/// Add a packet to the group that it is associated with
		/// </summary>
		/// <param name="packet">The packet to be added</param>
		/// <param name="packetCompleteHandle">The method to call and pass the data to when a sequence is complete</param>
		public void AddPacket(UDPPacket packet, BaseUDP.PacketComplete packetCompleteHandle, NetWorker networker)
		{
			// Check to see if we have already started this sequence
			if (!packetGroups.ContainsKey(packet.groupId))
				packetGroups.Add(packet.groupId, new UDPPacketGroup(packet.groupId));

			packetGroups[packet.groupId].AddPacket(packet, packetCompleteHandle, networker);
		}
	}
}
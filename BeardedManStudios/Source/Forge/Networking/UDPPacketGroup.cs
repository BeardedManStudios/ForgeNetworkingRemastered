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
	public class UDPPacketGroup
	{
		private const int MAX_ACCEPT_TIME_WINDOW = 30000;

		/// <summary>
		/// The list of sequences which are indexed by their timestamp
		/// </summary>
		private Dictionary<ulong, UDPPacketSequence> packets = new Dictionary<ulong, UDPPacketSequence>();

		private struct IdTracker
		{
			public ulong storeTime;
			public ulong id;
		}

		/// <summary>
		/// This is a list of packet ids that are pending removal
		/// </summary>
		private List<IdTracker> trackers = new List<IdTracker>();

		private TimeManager time = new TimeManager();

		public int GroupId { get; private set; }
		public UDPPacketGroup(int groupId)
		{
			GroupId = groupId;
		}

		/// <summary>
		/// Add a packet to a sequence based on the server timestamp
		/// </summary>
		/// <param name="packet">The packet to be added</param>
		/// <param name="packetCompleteHandle">The method to call and pass the data to when a sequence is complete</param>
		public void AddPacket(UDPPacket packet, BaseUDP.PacketComplete packetCompleteHandle, NetWorker networker)
		{
			// Don't process packets that have a timestep within a specified range
			//if (Time.Milliseconds - packet.timeStep > MAX_ACCEPT_TIME_WINDOW)
			//{
			// TODO:  Send an event for old message received or packet rejected
			//	return;
			//}

			// Removal of packets from this lookup is done on a separate thread
			lock (packets)
			{
				// Check to see if we have already started this sequence
				if (!packets.ContainsKey(packet.uniqueId))
					packets.Add(packet.uniqueId, new UDPPacketSequence());
			}

			// Cache the sequence so we don't repeatedly look it up
			UDPPacketSequence sequence = packets[packet.uniqueId];

			// Do not continue to add the packet if the sequence is already complete
			if (sequence.Done)
				return;

			if (sequence.AddPacket(packet))
			{
				// The packet sequence is complete
				CompleteSequence(packet.uniqueId, sequence, packetCompleteHandle, networker);
			}
		}

		/// <summary>
		/// Calls the supplied completion handler and passes the complete packet, then removes
		/// the sequence from the pending list 
		/// </summary>
		/// <param name="id">The timestamp for the packet to be used to lookup in packets dictionary</param>
		/// <param name="sequence">The actual sequence reference to skip another lookup</param>
		/// <param name="packetCompleteHandle">The method to call and pass this sequence into</param>
		private void CompleteSequence(ulong id, UDPPacketSequence sequence, BaseUDP.PacketComplete packetCompleteHandle, NetWorker networker)
		{
			packetCompleteHandle(sequence.GetData(networker), GroupId, (byte)sequence.Receivers, sequence.Reliable);

			lock (packets)
			{
				trackers.Add(new IdTracker() { storeTime = time.Timestep, id = id });
			}

			lock (packets)
			{
				for (int i = 0; i < trackers.Count; i++)
				{
					if (trackers[i].storeTime + MAX_ACCEPT_TIME_WINDOW <= time.Timestep)
					{
						packets.Remove(trackers[i].id);
						trackers.RemoveAt(i--);
					}
				}
			}
		}
	}
}
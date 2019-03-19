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
	public class UDPPacketSequence
	{
		/// <summary>
		/// Determines if this sequence of packets are reliable
		/// </summary>
		public bool Reliable { get; private set; }

		/// <summary>
		/// The id of the last packet in this sequence when found
		/// </summary>
		public int End { get; private set; }


		public Receivers Receivers { get; private set; }

		/// <summary>
		/// The list of packets that make up this sequence
		/// </summary>
		Dictionary<int, UDPPacket> packets = new Dictionary<int, UDPPacket>();

		/// <summary>
		/// Used to determine if this sequence is complete
		/// </summary>
		public bool Done { get; private set; }

		/// <summary>
		/// Adds the packet to the sequence
		/// </summary>
		/// <returns><c>true</c>, if packet copmelted the sequence, <c>false</c> otherwise.</returns>
		/// <param name="packet">The packet to be added</param>
		public bool AddPacket(UDPPacket packet)
		{
			// If there are no packets then this is the first packet, so we need
			// to do any initialization here related to the packets coming in
			if (packets.Count == 0)
				Reliable = packet.reliable;

			// Check to see if the packet has already been read
			if (packets.ContainsKey(packet.orderId))
				return false;

			// Check to see if this is the last packet in the list, if so store it
			// so that when other packets come in we can determine when to complete
			if (packet.endPacket)
			{
				End = packet.orderId + 1;
				Receivers = packet.receivers;
			}

			packets.Add(packet.orderId, packet);

			// Check to see if the end packet was received
			if (End != 0)
			{
				// If this dictionary has the same amount of packets as the end packet order id we are done
				if (packets.Count == End)
				{
					Done = true;
					return true;
				}
			}

			// This packet sequence is still incomplete
			return false;
		}

		/// <summary>
		/// Collect all of the data from all of the packets in this sequence and return it
		/// </summary>
		/// <returns>The complete packet sequence data</returns>
		public BMSByte GetData(NetWorker networker)
		{
			networker.PacketSequenceData.Clear();

			for (int i = 0; i < End; i++)
				networker.PacketSequenceData.BlockCopy(packets[i].rawBytes, 0, packets[i].rawBytes.Length);

			return networker.PacketSequenceData;
		}
	}
}
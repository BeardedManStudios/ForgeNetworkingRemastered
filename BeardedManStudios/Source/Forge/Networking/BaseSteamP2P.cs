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

#if STEAMWORKS
using BeardedManStudios.Forge.Networking.Frame;
using Steamworks;
using System;
using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking
{
	public abstract class BaseSteamP2P : NetWorker
	{
		public delegate void PacketComplete(BMSByte data, int groupId, byte receivers, bool isReliable);
		public delegate void MessageConfirmedEvent(NetworkingPlayer player, UDPPacket packet);

		public event MessageConfirmedEvent messageConfirmed;

		public CachedSteamP2PClient Client { get; protected set; }
        public CSteamID LobbyID { get; protected set; }

        protected List<SteamP2PPacketComposer> pendingComposers = new List<SteamP2PPacketComposer>();

		public BaseSteamP2P() { }
		public BaseSteamP2P(int maxConnections) : base(maxConnections) { }

		public abstract void Send(FrameStream frame, bool reliable = false);

		/// <summary>
		/// Used to clean up the target composer from memory
		/// </summary>
		/// <param name="composer">The composer that has completed</param>
		protected void ComposerCompleted(SteamP2PPacketComposer composer)
		{
#if DEEP_LOGGING
			Logging.BMSLog.Log($"<<<<<<<<<<<<<<<<<<<<<<<<<<< CONFIRMING: {composer.Frame.UniqueId}");
#endif

			lock (pendingComposers)
			{
				pendingComposers.Remove(composer);
			}
		}

		private byte PullPacketMetadata(BMSByte packet)
		{
			byte meta = packet.GetBasicType<byte>(packet.Size - sizeof(byte));
			packet.SetSize(packet.Size - sizeof(byte));
			return meta;
		}

		private int PullPacketOrderId(BMSByte packet)
		{
			// This assumes that packet metadata was pulled first
			int orderId = packet.GetBasicType<int>(packet.Size - sizeof(int));
			packet.SetSize(packet.Size - sizeof(int));
			return orderId;
		}

		private int PullPacketGroupId(BMSByte packet)
		{
			// This assumes that packet order id was pulled first
			int groupId = packet.GetBasicType<int>(packet.Size - sizeof(int));
			packet.SetSize(packet.Size - sizeof(int));
			return groupId;
		}

		private ulong PullPacketUniqueId(BMSByte packet, bool endPacket)
		{
			// This assumes that packet group id was pulled first
			ulong uniqueId = packet.GetBasicType<ulong>(packet.Size - sizeof(ulong));

			// Don't set the size like in the others unless it is the end packet 
			// because the frame will consume this time step The frame expects
			// the last bytes in the message to be the time step
			if (!endPacket)
				packet.SetSize(packet.Size - sizeof(ulong));

			return uniqueId;
		}

		protected UDPPacket TranscodePacket(NetworkingPlayer sender, BMSByte packet)
		{
			byte meta = PullPacketMetadata(packet);

			// If the last byte says it is reliable
			bool reliable = (0x1 & meta) != 0;

			// If the last byte says that it is the last packet for this group
			bool endPacket = (0x2 & meta) != 0;

			// If the last byte says that it is a conformation packet
			bool confirmationPacket = (0x4 & meta) != 0;

			// Get the receivers from the frist 4 bits
			Receivers receivers = (Receivers)(meta >> 4);

			// The group and order for this packet are assigned to the trailer of the packet, as
			// the header is reserved for frame formation
			int orderId = PullPacketOrderId(packet);
			int groupId = PullPacketGroupId(packet);

			ulong uniqueId = PullPacketUniqueId(packet, endPacket);

			// Check to see if this should respond to the sender that this packet has been received
			if (reliable && !confirmationPacket)
			{
#if DEEP_LOGGING
				Logging.BMSLog.Log($">>>>>>>>>>>>>>>>>>>>>>>>>>> SEND CONFIRM: {uniqueId}");
#endif

				byte[] confirmation = new byte[sizeof(ulong) + sizeof(int) + sizeof(int) + sizeof(byte)];
				Buffer.BlockCopy(BitConverter.GetBytes(uniqueId), 0, confirmation, 0, sizeof(ulong));
				Buffer.BlockCopy(BitConverter.GetBytes(groupId), 0, confirmation, sizeof(ulong), sizeof(int));
				Buffer.BlockCopy(BitConverter.GetBytes(orderId), 0, confirmation, sizeof(ulong) + sizeof(int), sizeof(int));

				// Register the confirmation flag in the last byte
				confirmation[confirmation.Length - 1] = (byte)(meta | 0x4);

				Client.Send(confirmation, confirmation.Length, sender.SteamID);
			}

			// Create an instance of a packet struct to be sent off to the packet manager
			UDPPacket formattedPacket = new UDPPacket(reliable, endPacket, groupId, orderId, uniqueId, packet.CompressBytes(), confirmationPacket, receivers);

			return formattedPacket;
		}

		/// <summary>
		/// A wrapper for the messageConfirmed event call that children of this can call
		/// </summary>
		protected void OnMessageConfirmed(NetworkingPlayer player, UDPPacket packet)
		{
			if (messageConfirmed != null)
				messageConfirmed(player, packet);
		}
	}
}
#endif
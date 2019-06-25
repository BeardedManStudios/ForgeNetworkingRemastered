﻿#if FACEPUNCH_STEAMWORKS
using BeardedManStudios.Forge.Networking.Frame;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class FacepunchP2PPacketComposer : BasePacketComposer
	{
		/// <summary>
		/// A base for any composer based events
		/// </summary>
		/// <param name="composer">The composer that fired off the event</param>
		public delegate void ComposerEvent(FacepunchP2PPacketComposer composer);

		/// <summary>
		/// Occurs when this composer has completed all of its messaging tasks
		/// </summary>
		public event ComposerEvent completed;

		/// <summary>
		/// The maximum size allowed for each packet
		/// </summary>
		public const ushort PACKET_SIZE = 1200;

		/// <summary>
		/// A reference to the client worker that this composer belongs to
		/// </summary>
		public BaseFacepunchP2P ClientWorker { get; private set; }

		/// <summary>
		/// The target player in question that will be receiving this data
		/// </summary>
		public NetworkingPlayer Player { get; private set; }

		/// <summary>
		/// If this message is reliable so that the object knows if it needs to attempt to resend packets
		/// </summary>
		public bool Reliable { get; private set; }

		/// <summary>
		/// The list of packets that are to be resent if it is reliable, otherwise it is just the
		/// list of packets that is to be sent and forgotten about
		/// </summary>
		public Dictionary<int, UDPPacket> PendingPackets { get; private set; }

		public FacepunchP2PPacketComposer() { }

		public FacepunchP2PPacketComposer(BaseFacepunchP2P clientWorker, NetworkingPlayer player, FrameStream frame, bool reliable = false)
		{
#if DEEP_LOGGING
			Logging.BMSLog.Log("---------------------------\n" + (new System.Diagnostics.StackTrace()).ToString() + "\nUNIQUE ID: " + frame.UniqueId.ToString() + "\n---------------------------");
#endif

			Init(clientWorker, player, frame, reliable);
		}

		/// <summary>
		/// Initialize the packet composer if the constructor with no arguments was used
		/// </summary>
		/// <param name="clientWorker">This BaseFacepunchP2P client worker</param>
		/// <param name="player">Networking player to whom we are sending data</param>
		/// <param name="frame">Data to be sent</param>
		/// <param name="reliable">Send reliable (slow) or unreliable (fast)</param>
		public void Init(BaseFacepunchP2P clientWorker, NetworkingPlayer player, FrameStream frame, bool reliable = false)
		{
			ClientWorker = clientWorker;
			Player = player;
			Frame = frame;
			Reliable = reliable;
			Initialize();
		}

		/// <summary>
		/// Send the packet off to the recipient
		/// </summary>
		/// <param name="data">The packet data that is to be sent</param>
		private void Send(byte[] data)
		{
			ClientWorker.Client.Send(data, data.Length, Player.SteamID, Reliable ? Steamworks.P2PSend.Reliable : Steamworks.P2PSend.Unreliable);
		}

		/// <summary>
		/// Set up the Packet Composer
		/// </summary>
		private void Initialize()
		{
			CreatePackets();

			// If this is a reliable message then we need to make sure to try and resend the message
			// at a given interval, later on this could be sent at the players last ping + time buffer
			if (Reliable)
			{
				// Make sure to register that this composer is to listen for completed packets to know
				// when each of the packets have been confirmed by the recipient
				ClientWorker.messageConfirmed += MessageConfirmed;

				Player.QueueComposer(this);
			}
			else
			{
				// TODO:  Probably should run this off the main thread
				// Go through all of the packets that were created and send them out immediately
				foreach (KeyValuePair<int, UDPPacket> kv in PendingPackets)
				{
					Send(kv.Value.rawBytes);

					ClientWorker.BandwidthOut += (ulong)kv.Value.rawBytes.Length;

					// Spread the packets apart by 1 ms to prevent any clobbering that may happen
					// on the socket layer for sending too much data
					Thread.Sleep(1);
				}

				Cleanup();
			}
		}

		/// <summary>
		/// Cleans up the thread, pending packets, and fires off any completion events
		/// </summary>
		private void Cleanup()
		{
			lock (PendingPackets)
			{
				PendingPackets.Clear();
			}

			if (completed != null)
				completed(this);
		}

		/// <summary>
		/// Go through all of the data and compile it into separated packets based on the PACKET_SIZE
		/// </summary>
		private void CreatePackets()
		{
			PendingPackets = new Dictionary<int, UDPPacket>();

			// Get all of the data that is available for this frame
			byte[] data = Frame.GetData(Reliable, Player);

			int byteIndex = 0, orderId = 0;

			byte[] trailer = new byte[9];

			Buffer.BlockCopy(BitConverter.GetBytes(Frame.GroupId), 0, trailer, 0, sizeof(int));

			if (Reliable)
				trailer[trailer.Length - 1] |= 0x1;

			do
			{
				int remainingPacketSize = data.Length - byteIndex + trailer.Length;
				bool endPacket = remainingPacketSize <= PACKET_SIZE;
				int length = 0;

				// We need to add the time step to this packet if it is not the end
				if (!endPacket)
				{
					// We need to backtrack the length of the added timestamp
					length -= sizeof(ulong);
					remainingPacketSize += -length;
				}

				// Create the packet space in memory and assign it to the correct length
				byte[] packet = new byte[Math.Min(PACKET_SIZE, remainingPacketSize)];

				length += packet.Length - trailer.Length;

				// Copy the bytes from the source into the new packet
				Buffer.BlockCopy(data, byteIndex, packet, 0, length);

				// Make sure we count every byte so we end the loop correctly and also so we know
				// if this is the last packet in the sequence
				byteIndex += length;

				if (endPacket)
				{
					trailer[trailer.Length - 1] |= 0x2;

					// Add the receivers to the end header byte
					trailer[trailer.Length - 1] |= (byte)(((int)Frame.Receivers) << 4);
				}
				else    // We need to copy the unique id into this message
					Buffer.BlockCopy(BitConverter.GetBytes(Frame.UniqueId), 0, packet, length, sizeof(ulong));

				// Set the order id for this packet in the trailer
				Buffer.BlockCopy(BitConverter.GetBytes(orderId), 0, trailer, sizeof(int), sizeof(int));

				// Copy the trailer to the end of the packet
				Buffer.BlockCopy(trailer, 0, packet, packet.Length - trailer.Length, trailer.Length);

				// Create and add the new packet to pending packets so that it can be sent out
				PendingPackets.Add(orderId, new UDPPacket(Reliable, endPacket, Frame.GroupId, orderId, Frame.UniqueId, packet, false, Frame.Receivers));
				orderId++;
			}
			while (byteIndex < data.Length);
		}

		/// <summary>
		/// Go through all of the pending packets and resend them
		/// </summary>
		public override void ResendPackets(ulong timestep, ref int counter)
		{
			lock (PendingPackets)
			{
				foreach (KeyValuePair<int, UDPPacket> kv in PendingPackets)
				{
					if (kv.Value.LastSentTimestep + (ulong)Player.RoundTripLatency > timestep)
						continue;

					if (counter <= 0)
					{
						kv.Value.UpdateTimestep(timestep);
						continue;
					}

					counter -= kv.Value.rawBytes.Length;

					kv.Value.DoingRetry(timestep);
					Send(kv.Value.rawBytes);
					ClientWorker.BandwidthOut += (ulong)kv.Value.rawBytes.Length;
				}
			}
		}

		/// <summary>
		/// This method is called when a packet is received and is a confirmation packet
		/// </summary>
		/// <param name="packet">The packet that was received</param>
		private void MessageConfirmed(NetworkingPlayer player, UDPPacket packet)
		{
			// Check to make sure that this packet was sent from this group
			if (packet.groupId != Frame.GroupId)
				return;

			// Check to make sure that the packet was sent from this composer
			if (packet.uniqueId != Frame.UniqueId)
				return;

			if (player != Player)
				return;

			lock (PendingPackets)
			{
				UDPPacket foundPacket;

				// Check to see if we already received a confirmation for this packet
				if (!PendingPackets.TryGetValue(packet.orderId, out foundPacket))
					return;

				player.RoundTripLatency = (int)(player.Networker.Time.Timestep - foundPacket.LastSentTimestep);

				// Remove the packet from pending so that it isn't sent again
				PendingPackets.Remove(packet.orderId);

				// All of the messages have been successfully confirmed, so we can remove the event listener
				if (PendingPackets.Count == 0)
				{
					ClientWorker.messageConfirmed -= MessageConfirmed;

					Cleanup();
					Player.EnqueueComposerToRemove(packet.uniqueId);
				}
			}
		}
	}
}
#endif

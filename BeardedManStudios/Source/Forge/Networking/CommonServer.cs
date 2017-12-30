using BeardedManStudios.Forge.Networking.Frame;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class CommonServerLogic
	{
		private NetWorker server;

		public CommonServerLogic(NetWorker server)
		{
			this.server = server;
		}

		public bool PlayerIsReceiver(NetworkingPlayer player, FrameStream frame, float proximityDistance, NetworkingPlayer skipPlayer = null)
		{
			// Don't send messages to a player who has not been accepted by the server yet
			if ((!player.Accepted && !player.PendingAccepted) || player == skipPlayer)
				return false;

			if (player == frame.Sender)
			{
				// Don't send a message to the sending player if it was meant for others
				if (frame.Receivers == Receivers.Others || frame.Receivers == Receivers.OthersBuffered || frame.Receivers == Receivers.OthersProximity)
					return false;
			}

			// Check to see if the request is based on proximity
			if (frame.Receivers == Receivers.AllProximity || frame.Receivers == Receivers.OthersProximity)
			{
				// If the target player is not in the same proximity zone as the sender
				// then it should not be sent to that player
				if (player.ProximityLocation.Distance(frame.Sender.ProximityLocation) > proximityDistance)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Checks all of the clients to see if any of them are timed out
		/// </summary>
		public void CheckClientTimeout(Action<NetworkingPlayer> timeoutDisconnect)
		{
			List<NetworkingPlayer> timedoutPlayers = new List<NetworkingPlayer>();
			while (server.IsBound)
			{
				server.IteratePlayers((player) =>
				{
					// Don't process the server during this check
					if (player == server.Me)
						return;

					if (player.TimedOut())
					{
						timedoutPlayers.Add(player);
					}
				});

				if (timedoutPlayers.Count > 0)
				{
					foreach (NetworkingPlayer player in timedoutPlayers)
						timeoutDisconnect(player);

					timedoutPlayers.Clear();
				}

				// Wait a second before checking again
				Thread.Sleep(1000);
			}
		}

		/// <summary>
		/// Disconnects a client
		/// </summary>
		/// <param name="client">The target client to be disconnected</param>
		public void Disconnect(NetworkingPlayer player, bool forced,
			List<NetworkingPlayer> DisconnectingPlayers, List<NetworkingPlayer> ForcedDisconnectingPlayers)
		{
			if (player.IsDisconnecting || DisconnectingPlayers.Contains(player) || ForcedDisconnectingPlayers.Contains(player))
				return;

			if (!forced)
				DisconnectingPlayers.Add(player);
			else
				ForcedDisconnectingPlayers.Add(player);
		}
	}
}
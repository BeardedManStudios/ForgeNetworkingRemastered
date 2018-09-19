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

            // check if sender is null as it doesn't get sent in certain cases
            if (frame.Sender != null)
            {
                // Check to see if the request is based on proximity
                if (frame.Receivers == Receivers.AllProximity || frame.Receivers == Receivers.OthersProximity)
                {
                    // If the target player is not in the same proximity zone as the sender
                    // then it should not be sent to that player
                    if (player.ProximityLocation.DistanceSquared(frame.Sender.ProximityLocation) > proximityDistance * proximityDistance)
                    {
                        return false;
                    }
                }
            }
			return true;
		}

        public bool PlayerIsTooFar(NetworkingPlayer sender, NetworkingPlayer player, FrameStream frame, float proximityDistance, float proximityModeUpdateFrequency)
        {
            // check for distance here so the owner doesn't need to be sent in stream, used for NCW field proximity check
            if (sender != null && (frame.Receivers == Receivers.AllProximity || frame.Receivers == Receivers.OthersProximity))
            {
                // If the target player is not in the same proximity zone as the sender
                // then it should not be sent to that player
                if (player.ProximityLocation.DistanceSquared(sender.ProximityLocation) > proximityDistance * proximityDistance)
                {
                    // if update frequency is 0, it shouldn't ever get updated while too far
                    if (proximityModeUpdateFrequency == 0)
                        return true;

                    // if player update counts are stored, increment or update and reset them, if not, store them starting with 0
                    if (sender.PlayersProximityUpdateCounters.ContainsKey(player.Ip))
                    {
                        if (sender.PlayersProximityUpdateCounters[player.Ip] < proximityModeUpdateFrequency)
                        {
                            sender.PlayersProximityUpdateCounters[player.Ip]++;
                            return true;
                        }
                        else
                            sender.PlayersProximityUpdateCounters[player.Ip] = 0;
                    }
                    else
                        sender.PlayersProximityUpdateCounters.Add(player.Ip, 0);
                }
            }

            return false;
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
using BeardedManStudios.Forge.Networking;
using System;
using System.Text;

namespace BasicStandAloneServer
{
	class Program
	{
		private static void Main(string[] args)
		{
			int playerCount = 32;
			UDPServer networkHandle = new UDPServer(playerCount);
			networkHandle.textMessageReceived += ReadTextFrame;
			networkHandle.binaryMessageReceived += ReadBinaryFrame;
			networkHandle.playerAccepted += PlayerAccepted;

			networkHandle.Connect();

			while (true)
			{
				if (Console.ReadLine().ToLower() == "exit")
				{
					break;
				}
			}

			networkHandle.Disconnect(false);
		}

		private static void PlayerAccepted(NetworkingPlayer player, NetWorker sender)
		{
			Console.WriteLine($"New player accepted with id {player.NetworkId}");
		}

		private static void ReadBinaryFrame(NetworkingPlayer player, BeardedManStudios.Forge.Networking.Frame.Binary frame, NetWorker sender)
		{
			StringBuilder message = new StringBuilder(frame.StreamData.byteArr.Length);
			message.Append("Bytes: ");
			foreach (byte b in frame.StreamData.byteArr)
			{
				if (message.Length > 0)
					message.Append(',');

				message.Append(b.ToString());
			}

			Console.WriteLine(message);
		}

		private static void ReadTextFrame(NetworkingPlayer player, BeardedManStudios.Forge.Networking.Frame.Text frame, NetWorker sender)
		{
			Console.WriteLine("Read: " + frame.ToString());
		}
	}
}

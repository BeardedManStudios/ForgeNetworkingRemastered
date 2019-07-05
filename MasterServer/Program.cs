using System;
using System.Net;
using System.Net.Sockets;

namespace MasterServer
{
	internal static class Program
	{
		private static bool isDaemon = false;
		private static string host = "0.0.0.0";
		private static ushort port = 15940;
		private static int eloRange = 0;

		private static MasterServer server;

		private static void Main(string[] args)
		{
			string host = "0.0.0.0";
			ushort port = 15940;
			string read = string.Empty;
			int eloRange = 0;

			var options = new CommandLineOptions();
			if (CommandLine.Parser.Default.ParseArguments(args, options))
			{
				ushort.TryParse(options.Port, out port);
				host = options.Host;
				eloRange = options.EloRange;
				isDaemon = options.IsDaemon;
			}
			else
			{
				Console.WriteLine("Entering nothing will choose defaults.");
				Console.WriteLine("Enter Host IP (Default: " + GetLocalIpAddress() + "):");
				read = Console.ReadLine();
				host = string.IsNullOrEmpty(read) ? GetLocalIpAddress() : read;

				Console.WriteLine("Enter Port (Default: 15940):");
				read = Console.ReadLine();
				if (string.IsNullOrEmpty(read))
					port = 15940;
				else
					ushort.TryParse(read, out port);
			}

			Console.WriteLine("Hosting ip [{0}] on port [{1}]", host, port);
			PrintHelp();

			server = new MasterServer(host, port)
			{
				EloRange = eloRange
			};
			server.ToggleLogging();

			while (true)
			{
				if (!isDaemon)
					HandleConsoleInput();
			}
		}

		private static void HandleConsoleInput()
		{
			string read = Console.ReadLine();
			read = string.IsNullOrEmpty(read) ? read : read.ToLower();

			switch (read)
			{
				case null:
					return;

				case "s":
				case "stop":
					lock (server)
					{
						Console.WriteLine("Server stopped.");
						server.Dispose();
					}
					break;

				case "l":
				case "log":
					if (server.ToggleLogging())
						Console.WriteLine("Logging has been enabled");
					else
						Console.WriteLine("Logging has been disabled");
					break;

				case "r":
				case "restart":
					lock (server)
					{
						if (server.IsRunning)
						{
							Console.WriteLine("Server stopped.");
							server.Dispose();
						}
					}

					Console.WriteLine("Restarting...");
					Console.WriteLine("Hosting ip [{0}] on port [{1}]", host, port);
					server = new MasterServer(host, port);
					break;

				case "q":
				case "quit":
					lock (server)
					{
						Console.WriteLine("Quitting...");
						server.Dispose();
					}

					break;

				case "h":
				case "help":
					PrintHelp();
					break;

				default:
					if (read.StartsWith("elo"))
					{
						int index = read.IndexOf("=", StringComparison.Ordinal);
						string val = read.Substring(index + 1, read.Length - (index + 1));
						if (int.TryParse(val.Replace(" ", string.Empty), out index))
						{
							Console.WriteLine("Elo range set to {0}", index);
							if (index == 0)
								Console.WriteLine("Elo turned off");
							server.EloRange = index;
						}
						else
							Console.WriteLine("Invalid elo range provided (Must be an integer)\n");
					}
					else
						Console.WriteLine("Command not recognized, please try again");

					break;
			}
		}

		private static void PrintHelp()
		{
			Console.WriteLine(@"Commands Available
(s)top - Stops hosting
(r)estart - Restarts the hosting service even when stopped
(l)og - Toggles logging (starts enabled)
(q)uit - Quits the application
(h)elp - Get a full list of commands");
		}

	    /// <summary>
	    /// Return the Local IP-Address
	    /// </summary>
	    /// <returns></returns>
	    private static string GetLocalIpAddress()
	    {
	        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
	        foreach (IPAddress ip in hostEntry.AddressList)
	        {
	            if (ip.AddressFamily == AddressFamily.InterNetwork)
		            return ip.ToString();
	        }

	        throw new Exception("No network adapters with an IPv4 address in the system!");
	    }
    }
}

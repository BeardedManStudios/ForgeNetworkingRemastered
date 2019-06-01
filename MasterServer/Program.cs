using BeardedManStudios;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MasterServer
{
	internal static class Program
	{
		private static string s_Host = "0.0.0.0";
		private static ushort s_Port = 15940;
		private static string s_Read = string.Empty;
		private static int s_EloRange = 0;

		private static MasterServer s_Server;

		private static void Main(string[] args)
		{
			ParseArgs(args);

			Console.WriteLine("Hosting ip [{0}] on port [{1}]", s_Host, s_Port);
			PrintHelp();

			s_Server = new MasterServer(s_Host, s_Port)
			{
				EloRange = s_EloRange
			};
			s_Server.ToggleLogging();

			while (true)
				HandleConsoleInput();
		}

		private static void ParseArgs(string[] args)
		{
			Dictionary<string, string> arguments = ArgumentParser.Parse(args);

			if (args.Length > 0)
			{
				string value;
				if (arguments.TryGetValue("h", out value) || arguments.TryGetValue("host", out value))
					s_Host = value;

				if (arguments.TryGetValue("p", out value) || arguments.TryGetValue("port", out value))
					ushort.TryParse(value, out s_Port);

				if (arguments.TryGetValue("e", out value) || arguments.TryGetValue("elorange", out value))
					int.TryParse(value, out s_EloRange);
			}
			else
			{
				Console.WriteLine("Entering nothing will choose defaults.");
				Console.WriteLine("Enter Host IP (Default: " + GetLocalIpAddress() + "):");
				s_Read = Console.ReadLine();
				s_Host = string.IsNullOrEmpty(s_Read) ? GetLocalIpAddress() : s_Read;

				Console.WriteLine("Enter Port (Default: 15940):");
				s_Read = Console.ReadLine();
				if (string.IsNullOrEmpty(s_Read))
					s_Port = 15940;
				else
					ushort.TryParse(s_Read, out s_Port);
			}
		}

		private static void HandleConsoleInput()
		{
			string read = Console.ReadLine()?.ToLower();

			switch (read)
			{
				case null:
					return;

				case "s":
				case "stop":
					lock (s_Server)
					{
						Console.WriteLine("Server stopped.");
						s_Server.Dispose();
					}
					break;

				case "l":
				case "log":
					if (s_Server.ToggleLogging())
						Console.WriteLine("Logging has been enabled");
					else
						Console.WriteLine("Logging has been disabled");
					break;

				case "r":
				case "restart":
					lock (s_Server)
					{
						if (s_Server.IsRunning)
						{
							Console.WriteLine("Server stopped.");
							s_Server.Dispose();
						}
					}

					Console.WriteLine("Restarting...");
					Console.WriteLine("Hosting ip [{0}] on port [{1}]", s_Host, s_Port);
					s_Server = new MasterServer(s_Host, s_Port);
					break;
				
				case "q":
				case "quit":
					lock (s_Server)
					{
						Console.WriteLine("Quitting...");
						s_Server.Dispose();
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
							s_Server.EloRange = index;
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
	        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
	        foreach (IPAddress ip in host.AddressList)
	        {
	            if (ip.AddressFamily == AddressFamily.InterNetwork)
		            return ip.ToString();
	        }

	        throw new Exception("No network adapters with an IPv4 address in the system!");
	    }
    }
}

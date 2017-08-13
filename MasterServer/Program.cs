using BeardedManStudios;
using System.Collections.Generic;

namespace MasterServer
{
	class Program
	{
		static void Main(string[] args)
		{
			string host = "0.0.0.0";
			ushort port = 15940;
			string read = string.Empty;
			int eloRange = 0;

			Dictionary<string, string> arguments = ArgumentParser.Parse(args);

			if (args.Length > 0)
			{
				if (arguments.ContainsKey("host"))
					host = arguments["host"];

				if (arguments.ContainsKey("port"))
					ushort.TryParse(arguments["port"], out port);

				if (arguments.ContainsKey("elorange"))
					int.TryParse(arguments["elorange"], out eloRange);
			}
			else
			{
				System.Console.WriteLine("Entering nothing will choose defaults.");
				System.Console.WriteLine("Enter Host IP (Default: 0.0.0.0):");
				read = System.Console.ReadLine();
				if (string.IsNullOrEmpty(read))
					host = "0.0.0.0";

				System.Console.WriteLine("Enter Port (Default: 15940):");
				read = System.Console.ReadLine();
				if (string.IsNullOrEmpty(read))
					port = 15940;
				else
				{
					ushort.TryParse(read, out port);
				}
			}

			System.Console.WriteLine(string.Format("Hosting ip [{0}] on port [{1}]", host, port));
			System.Console.WriteLine("Commands Available\n(s)top - Stops hosting\n(r)estart - Restarts the hosting service even when stopped\n(q)uit - Quits the application\n(h)elp - Get a full list of comands");
			MasterServer server = new MasterServer(host, port);
			server.EloRange = eloRange;

			while (true)
			{
				read = System.Console.ReadLine().ToLower();
				if (read == "s" || read == "stop")
				{
					lock (server)
					{
						System.Console.WriteLine("Server stopped.");
						server.Dispose();
					}
				}
				else if (read == "r" || read == "restart")
				{
					lock (server)
					{
						if (server.IsRunning)
						{
							System.Console.WriteLine("Server stopped.");
							server.Dispose();
						}
					}

					System.Console.WriteLine("Restarting...");
					System.Console.WriteLine(string.Format("Hosting ip [{0}] on port [{1}]", host, port));
					server = new MasterServer(host, port);
				}
				else if (read == "q" || read == "quit")
				{
					lock (server)
					{
						System.Console.WriteLine("Quitting...");
						server.Dispose();
					}
					break;
				}
				else if (read == "h" || read == "help")
				{
					System.Console.WriteLine("(s)top - Stops hosting\n(r)estart - Restarts the hosting service even when stopped\n(e)lo - Set the elo range to accept in difference [i.e. \"elorange = 10\"]\n(q)uit - Quits the application\n(h)elp - Get a full list of comands");
				}
				else if (read.StartsWith("elo"))
				{
					int index = read.IndexOf("=");
					string val = read.Substring(index + 1, read.Length - (index + 1));
					if (int.TryParse(val.Replace(" ", string.Empty), out index))
					{
						System.Console.WriteLine(string.Format("Elo range set to {0}", index));
						if (index == 0)
							System.Console.WriteLine("Elo turned off");
						server.EloRange = index;
					}
					else
						System.Console.WriteLine("Invalid elo range provided (Must be an integer)\n");
				}
			}
		}
	}
}
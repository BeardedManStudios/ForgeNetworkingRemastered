using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace BeardedManStudios.Forge.Networking
{
	public static class HostResolver
	{
		private static readonly string[] localHosts = new string[]
		{
			"127.0.0.1", "0.0.0.0", "::0"
		};

		public static IPEndPoint Resolve(string host, ushort port)
		{
			// Check for any localhost type addresses
			if (localHosts.Contains(host))
				return new IPEndPoint(IPAddress.Parse(host), port);
			else if (host == "localhost")
				return new IPEndPoint(IPAddress.Parse(localHosts[0]), port);

			IPAddress ipAddress;
			if (!IPAddress.TryParse(host, out ipAddress))
			{
				if (TryResolveInterNetwork(host, port, out var endpoint))
				{
					return endpoint;
				}

				ipAddress = ResolveDomainName(host);
			}

			return new IPEndPoint(ipAddress, port);
		}

		private static bool TryResolveInterNetwork(string host, ushort port, out IPEndPoint endpoint)
		{
			endpoint = default;
			IPHostEntry hostCheck = Dns.GetHostEntry(Dns.GetHostName());

			foreach (IPAddress ip in hostCheck.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					if (ip.ToString() == host)
					{
						endpoint = new IPEndPoint(IPAddress.Parse(localHosts[0]), port);
						return true;
					}
				}
			}

			return false;
		}

		private static IPAddress ResolveDomainName(string host)
		{
			IPAddress ipAddress;

			try
			{
				IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
				ipAddress = ipHostInfo.AddressList[0];
			}
			catch
			{
				Logging.BMSLog.Log("Failed to find host");
				throw new ArgumentException("Unable to resolve host");
			}

			return ipAddress;
		}
	}
}

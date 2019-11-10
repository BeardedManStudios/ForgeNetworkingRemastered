using System.Net;
using System.Net.Sockets;

namespace Forge.Networking.Sockets
{
	public class CommonSocketBase
	{
		protected static IPEndPoint GetEndpoint(string address, ushort port)
		{
			string host = string.IsNullOrEmpty(address) ? Dns.GetHostName() : address;
			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);

			// TODO:  Support IPv6
			IPAddress ipAddress = null;
			for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)
			{
				ipAddress = ipHostInfo.AddressList[i];
				if (ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
				{
					break;
				}
			}

			var localEndPoint = new IPEndPoint(ipAddress, port);
			return localEndPoint;
		}
	}
}

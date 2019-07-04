using System.Net;
using System.Net.Sockets;

namespace BeardedManStudios.Source.Forge.Networking
{
    public static class LocalNetworkScanning
    {
        /// <summary>
		/// Get the local Ip address
		/// </summary>
		/// <returns>The Local Ip Address</returns>
		public static string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && IsPrivateIP(ip)) // JM: check for all local ranges
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            return localIP;
        }

        private static bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (myIPAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();

                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

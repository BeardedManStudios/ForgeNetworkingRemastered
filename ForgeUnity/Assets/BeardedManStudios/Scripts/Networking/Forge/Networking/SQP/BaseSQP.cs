using System.Net;
using System.Net.Sockets;

namespace BeardedManStudios.Forge.Networking.SQP
{
	public class BaseSQP
	{
		/// <summary>
		/// After how many milliseconds should a query be considered timed out.
		/// </summary>
		public const int QUERY_TIMEOUT = 3000;
		public const int MAX_PACKET_SIZE = 1472;

		protected Socket socket;
		protected BMSByte buffer = new BMSByte();
		protected EndPoint endpoint = new IPEndPoint(0, 0);

		public void ShutDown()
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}
		}

		/// <summary>
		/// Initialize the socket
		/// </summary>
		/// <param name="ep"></param>
		protected virtual void InitSocket(EndPoint ep)
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Blocking = false;

			socket.Bind(ep);

			buffer.SetSize(MAX_PACKET_SIZE);
		}
	}
}

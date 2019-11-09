using Forge.Serialization;
using System.Net;
using System.Net.Sockets;

namespace Forge.Networking.Sockets
{
	public class UDPSocket : ISocket, IServerSocket, IClientSocket
	{
		private readonly Socket _socket;

		public UDPSocket()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
		}

		public UDPSocket(Socket socket)
		{
			_socket = socket;
		}

		public ISocket AwaitAccept()
		{
			throw new System.NotImplementedException();
		}

		public void Close()
		{
			_socket.Close();
		}

		public void Connect(string address, ushort port)
		{
			var endpoint = GetEndpoint(address, port);
			_socket.Connect(endpoint);
		}

		public void Listen(string address, ushort port, int maxParallelConnections)
		{
			var endpoint = GetEndpoint(address, port);
			_socket.Bind(endpoint);
			_socket.Listen(maxParallelConnections);
		}

		public int Receive(BMSByte buffer)
		{
			buffer.Clear();
			int length = _socket.Receive(buffer.byteArr);
			buffer.AugmentSize(length);
			return length;
		}

		public void Send(byte[] buffer, int length)
		{
			int offset = 0;
			_socket.Send(buffer, offset, length, SocketFlags.None);
		}

		private IPEndPoint GetEndpoint(string address, ushort port)
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

			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
			return localEndPoint;
		}
	}
}

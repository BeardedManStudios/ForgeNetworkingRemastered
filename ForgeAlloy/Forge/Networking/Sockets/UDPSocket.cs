using System.Net;
using System.Net.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class UDPSocket : ISocket, IServerSocket, IClientSocket
	{
		public EndPoint EndPoint { get; private set; }

		private BMSByte _acceptBuffer = new BMSByte();

		private readonly Socket _acceptSocket;
		private readonly Socket _liveSocket;

		public UDPSocket()
		{
			_acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
			_liveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
			_acceptBuffer.SetArraySize(256);
		}

		public UDPSocket(Socket socket)
		{
			_acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
			_acceptBuffer.SetArraySize(256);
			_liveSocket = socket;
		}

		public ISocket AwaitAccept()
		{
			EndPoint ep = default;
			var sock = new UDPSocket();
			_acceptSocket.ReceiveFrom(_acceptBuffer.byteArr, 0, _acceptBuffer.Size, SocketFlags.None, ref ep);
			sock.EndPoint = ep;
			return sock;
		}

		public void Close()
		{
			_acceptSocket.Close();
			_liveSocket.Close();
		}

		public void Connect(string address, ushort port)
		{
			EndPoint = GetEndpoint(address, port);
			_liveSocket.Connect(EndPoint);
		}

		public void Listen(string address, ushort port, int maxParallelConnections)
		{
			var endpoint = GetEndpoint(address, port);
			_acceptSocket.Bind(endpoint);
			_acceptSocket.Listen(maxParallelConnections);
		}

		public int Receive(BMSByte buffer)
		{
			buffer.Clear();
			int length = _liveSocket.Receive(buffer.byteArr);
			buffer.AugmentSize(length);
			return length;
		}

		public void Send(ISocket target, byte[] buffer, int length)
		{
			int offset = 0;
			_liveSocket.SendTo(buffer, offset, length, SocketFlags.None, target.EndPoint);
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

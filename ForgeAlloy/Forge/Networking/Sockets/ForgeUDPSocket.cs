using System.Net;
using System.Net.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocket : IServerSocket, IClientSocket
	{
		public EndPoint EndPoint { get; private set; } = default;

		private BMSByte _acceptBuffer = new BMSByte();

		private readonly Socket _acceptSocket;
		private readonly Socket _liveSocket;

		public ForgeUDPSocket()
		{
			_acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_liveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_acceptBuffer.SetArraySize(256);
		}

		private ForgeUDPSocket(Socket socket)
		{
			_acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_acceptBuffer.SetArraySize(256);
			_liveSocket = socket;
		}

		public ISocket AwaitAccept()
		{
			EndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15937);
			var sock = new ForgeUDPSocket();
			_acceptSocket.ReceiveFrom(_acceptBuffer.byteArr, 0, _acceptBuffer.byteArr.Length, SocketFlags.None, ref ep);
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

		public void Listen(ushort port, int maxParallelConnections)
		{
			var endpoint = new IPEndPoint(IPAddress.Any, port);
			_acceptSocket.Bind(endpoint);
			var liveEndpoint = new IPEndPoint(IPAddress.Any, (ushort)(port + 1));
			_liveSocket.Bind(liveEndpoint);
			EndPoint = _liveSocket.LocalEndPoint;
		}

		public int Receive(BMSByte buffer, ref EndPoint endpoint)
		{
			buffer.Clear();
			int length = _liveSocket.ReceiveFrom(buffer.byteArr, 0, buffer.byteArr.Length, SocketFlags.None, ref endpoint);
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

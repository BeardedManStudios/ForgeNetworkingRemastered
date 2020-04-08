using System.Net;
using System.Net.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class ForgeUDPSocket : CommonSocketBase, IServerSocket, IClientSocket
	{
		public EndPoint EndPoint { get; private set; } = default;

		private readonly Socket _socket;

		public ForgeUDPSocket()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		}

		private ForgeUDPSocket(Socket socket)
		{
			_socket = socket;
		}

		public void Close()
		{
			_socket.Close();
		}

		public void Connect(string address, ushort port)
		{
			EndPoint = GetEndpoint(address, port);
			_socket.Connect(EndPoint);
		}

		public void Listen(ushort port, int maxParallelConnections)
		{
			var endpoint = new IPEndPoint(IPAddress.Any, port);
			_socket.Bind(endpoint);
			EndPoint = _socket.LocalEndPoint;
		}

		public int Receive(BMSByte buffer, ref EndPoint endpoint)
		{
			buffer.Clear();
			int length = _socket.ReceiveFrom(buffer.byteArr, 0, buffer.byteArr.Length, SocketFlags.None, ref endpoint);
			buffer.AugmentSize(length);
			return length;
		}

		public void Send(EndPoint endpoint, BMSByte buffer)
		{
			int offset = 0;
			_socket.SendTo(buffer.byteArr, offset, buffer.Size, SocketFlags.None, endpoint);
		}
	}
}

using System;
using System.Net;
using System.Net.Sockets;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class ForgeTCPSocket : CommonSocketBase, IServerSocket, IClientSocket
	{
		public EndPoint EndPoint => _socket.RemoteEndPoint;

		private readonly Socket _socket;

		public ForgeTCPSocket()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		private ForgeTCPSocket(Socket socket)
		{
			_socket = socket;
		}

		public void Connect(string address, ushort port)
		{
			var endpoint = GetEndpoint(address, port);
			_socket.Connect(endpoint);
		}

		public void Listen(ushort port, int maxParallelConnections)
		{
			var endpoint = new IPEndPoint(IPAddress.Any, port);
			_socket.Bind(endpoint);
			_socket.Listen(maxParallelConnections);
		}

		public void Close()
		{
			_socket.Close();
		}

		public ISocket AwaitAccept()
		{
			var newConnection = _socket.Accept();
			return new ForgeTCPSocket(newConnection);
		}

		public int Receive(BMSByte buffer, ref EndPoint endpoint)
		{
			buffer.Clear();
			int length = _socket.ReceiveFrom(buffer.byteArr, ref endpoint);
			buffer.AugmentSize(length);
			return length;
		}

		public void Send(EndPoint endpoint, BMSByte buffer)
		{
			int offset = 0;
			_socket.SendTo(buffer.byteArr, offset, buffer.Size, SocketFlags.None, endpoint);
		}

		public void ReportAcceptance(ISocket target)
		{
			byte[] portBytes = BitConverter.GetBytes(((IPEndPoint)EndPoint).Port);
			_socket.SendTo(portBytes, 0, portBytes.Length, SocketFlags.None, target.EndPoint);
		}
	}
}

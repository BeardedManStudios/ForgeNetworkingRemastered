using System.Net;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public interface ISocket
	{
		EndPoint EndPoint { get; }
		void Send(ISocket target, byte[] buffer, int length);
		int Receive(BMSByte buffer);
		void Close();
	}
}

using System.Net;

namespace Forge.Networking.Sockets
{
	public class SocketContainerSynchronizationReadData
	{
		public byte[] Buffer { get; set; }
		public EndPoint Endpoint { get; set; }
	}
}

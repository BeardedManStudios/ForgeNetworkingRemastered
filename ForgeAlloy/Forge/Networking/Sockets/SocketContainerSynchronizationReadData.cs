using System.Net;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public class SocketContainerSynchronizationReadData
	{
		public BMSByte Buffer { get; set; }
		public EndPoint Endpoint { get; set; }
	}
}

using System.Net;
using Forge.Networking.Messaging;

namespace Forge.Networking.Sockets
{
	public class SocketContainerSynchronizationReadData
	{
		public IMessageInterpreter Interpreter { get; set; }
		public EndPoint Sender { get; set; }
		public IMessage Message { get; set; }
	}
}

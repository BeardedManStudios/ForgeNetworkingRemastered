using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.TCP
{
	[TestFixture]
	public class TCPClientTests : BaseTest
	{
		private UDPServer server = null;

		[SetUp]
		public void CreateServer()
		{
			server = new UDPServer(32);
			server.Connect();
		}

		[TearDown]
		public void DisposeServer()
		{
			server.Disconnect(false);
		}
	}
}

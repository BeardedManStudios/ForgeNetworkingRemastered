using BeardedManStudios.Forge.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.TCP
{
	[TestClass]
	public class TCPClientTests : BaseTest
	{
		UDPServer server = null;

		[TestInitialize]
		public void CreateServer()
		{
			server = new UDPServer(32);
			server.Connect();
		}

		[TestCleanup]
		public void DisposeServer()
		{
			server.Disconnect(false);
		}
	}
}
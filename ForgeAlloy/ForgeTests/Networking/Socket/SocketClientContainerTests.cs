using System.Threading;
using NUnit.Framework;

namespace ForgeTests.Networking.Socket
{
	[TestFixture]
	public class SocketClientContainerTests : ForgeNetworkingTest
	{
		public override void Setup()
		{
			base.Setup();
			SynchronizationContext.SetSynchronizationContext(new ForgeTestSynchronizationContext());
		}
	}
}

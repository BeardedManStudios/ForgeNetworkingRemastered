using System.Threading;
using NUnit.Framework;

namespace ForgeTests.Networking.Socket
{
	[TestFixture]
	public class SocketClientContainerTests : ForgeNetworkingTest
	{
		// TODO:  Create tests for ForgeNetworkingTest

		public override void Setup()
		{
			base.Setup();
			SynchronizationContext.SetSynchronizationContext(new ForgeTestSynchronizationContext());
		}
	}
}

using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests
{
	[TestFixture]
	public class NetworkInterfaceAccumulatorTests
	{
		[Test]
		public void CheckToMakeSureSomethingExistsOnTheNetworkIPv4()
		{
			// Our computer should be listed on the network
			var a = new NetworkInterfaceAccumulator();
			a.Accumulate(System.Net.Sockets.AddressFamily.InterNetwork);
			Assert.AreNotEqual(0, a.IPs.Count);
		}

		[Test]
		public void CheckToMakeSureSomethingExistsOnTheNetworkIPv6()
		{
			// Our computer should be listed on the network
			var a = new NetworkInterfaceAccumulator();
			a.Accumulate(System.Net.Sockets.AddressFamily.InterNetworkV6);
			Assert.AreNotEqual(0, a.IPs.Count);
		}
	}
}

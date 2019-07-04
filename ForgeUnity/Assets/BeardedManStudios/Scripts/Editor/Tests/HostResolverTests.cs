using BeardedManStudios.Forge.Networking;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests
{
	[TestFixture]
	public class HostResolverTests
	{
		private const string GOOGLE = "google.com";
		private const string LOCAL_IPV4 = "127.0.0.1";
		private const string LOCAL_IPV6 = "::0";
		private const string LOCAL_HOST = "localhost";
		private const ushort PORT = 15937;

		[Test]
		public void TestLocalIPv4()
		{
			var endpoint = HostResolver.Resolve(LOCAL_IPV4, PORT);
			Assert.NotNull(endpoint);
			Assert.AreEqual($"{LOCAL_IPV4}:{PORT}", endpoint.ToString());
		}

		[Test]
		public void TestLocalIPv6()
		{
			var endpoint = HostResolver.Resolve(LOCAL_IPV6, PORT);
			Assert.NotNull(endpoint);
			Assert.AreEqual($"[::]:{PORT}", endpoint.ToString());
		}

		[Test]
		public void TestLocalhost()
		{
			var endpoint = HostResolver.Resolve(LOCAL_HOST, PORT);
			Assert.NotNull(endpoint);
			Assert.AreEqual($"{LOCAL_IPV4}:{PORT}", endpoint.ToString());
		}

		[Test]
		public void TestGoogle()
		{
			var endpoint = HostResolver.Resolve(GOOGLE, 80);
			Assert.NotNull(endpoint);
			Assert.False(string.IsNullOrEmpty(endpoint.ToString()));
		}
	}
}

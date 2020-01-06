using Forge.Networking.Sockets;
using Forge.ServerRegistry.DataStructures;
using Forge.ServerRegistry.Serializers;
using NUnit.Framework;

namespace ForgeTests.Serialization.Serializers
{
	[TestFixture]
	public class ServerListingEntryArraySerializerTests : TypeSerializerTestsBase
	{
		[Test]
		public void SerializingServerListingEntryArray_ShouldBeEqual()
		{
			ServerListingEntry[] input = new ServerListingEntry[]
			{
				new ServerListingEntry
				{
					 Address = "10.2.5.96",
					 Port = 1265
				},
				new ServerListingEntry
				{
					 Address = CommonSocketBase.LOCAL_IPV4,
					 Port = 15937
				},
				new ServerListingEntry
				{
					 Address = CommonSocketBase.LOCAL_ANY_IPV4,
					 Port = 35719
				},
				new ServerListingEntry
				{
					 Address = "76.36.45.25",
					 Port = 555
				},
				new ServerListingEntry
				{
					 Address = "255.0.2.255",
					 Port = 983
				}
			};
			ServerListingEntry[] res = SerializeDeserialize(new ServerListingEntryArraySerializer(), input);
			Assert.AreEqual(input.Length, res.Length);
			for (int i = 0; i < input.Length; i++)
			{
				Assert.AreEqual(input[i].Address, res[i].Address);
				Assert.AreEqual(input[i].Port, res[i].Port);
			}
		}
	}
}

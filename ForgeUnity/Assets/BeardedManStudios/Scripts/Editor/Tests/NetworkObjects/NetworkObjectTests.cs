using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.NetworkObjects
{
	[TestFixture]
	public class NetworkObjectTests : BaseNetworkObjectTest
	{
		[OneTimeSetUp]
		public static void CreateServer()
		{
			ConnectSetup();

			NetworkObject.Factory = new NetworkObjectFactory();
			NetworkObject.Flush(client);
		}

		[OneTimeTearDown]
		public static void DisposeServer()
		{
			ConnectTeardown();
		}

		[SetUp]
		public void Startup()
		{
			// Capture when an object is created and assign it to the static reference
			server.objectCreated += ObjectCreated;
			client.objectCreated += ObjectCreated;
		}

		[TearDown]
		public void Cleanup()
		{
			server.objectCreated -= ObjectCreated;
			client.objectCreated -= ObjectCreated;

			serverObj = null;
			clientObj = null;
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void TestCreate()
		{
			SetupServerBehavior();

			// Make sure that the client get's the network object that was created
			WaitFor(() => { return clientObj != null; });

			Destroy(serverObj);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void TestServerTakeOwnership()
		{
			SetupClientBehavior();

			// Make the server take ownership of the object from the client
			serverObj.TakeOwnership();
			WaitFor(() => { return serverObj.IsOwner; });

			// Check to make sure that the server actually has ownership by changing the position
			// and making sure the client's network object reflects that change
			serverObj.fieldFloat = 9.34f;
			WaitFor(() => { return clientObj.fieldFloat == 9.34f; });

			// The server took ownership so it should be the one to destroy it
			Destroy(serverObj);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void TestClientTakeOwnership()
		{
			SetupServerBehavior();

			// Make the client request to take ownership of the object from the server
			clientObj.TakeOwnership();
			WaitFor(() => { return clientObj.IsOwner; });

			// Check to make sure that the client actually has ownership by changing the position
			// and making sure the server's network object reflects that change
			clientObj.fieldFloat = 9.34f;
			WaitFor(() => { return serverObj.fieldFloat == 9.34f; });

			Destroy(serverObj);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void TestServerGiveOwnership()
		{
			SetupServerBehavior();

			// Have the server give the ownership to the client
			serverObj.AssignOwnership(server.Players.Last());
			WaitFor(() => { return clientObj.IsOwner; });

			// Check to make sure that the server actually has ownership by changing the position
			// and making sure the client's network object reflects that change
			clientObj.fieldFloat = 9.34f;
			WaitFor(() => { return serverObj.fieldFloat == 9.34f; });

			Destroy(serverObj);
		}
	}
}

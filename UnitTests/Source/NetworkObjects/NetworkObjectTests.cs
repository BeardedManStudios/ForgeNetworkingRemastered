using BeardedManStudios.Forge.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UnitTests.NetworkObjectSetup;

namespace UnitTests.NetworkObjects
{
	[TestClass]
	public class NetworkObjectTests : BaseNetworkObjectTest
	{
		[ClassInitialize]
		public static void CreateServer(TestContext context)
		{
			ConnectSetup();

			NetworkObject.Factory = new NetworkObjectFactory();
			NetworkObject.Flush(client);
		}

		[ClassCleanup]
		public static void DisposeServer()
		{
			ConnectTeardown();
		}

		[TestInitialize]
		public void Startup()
		{
			// Capture when an object is created and assign it to the static reference
			server.objectCreated += ObjectCreated;
			client.objectCreated += ObjectCreated;
		}

		[TestCleanup]
		public void Cleanup()
		{
			server.objectCreated -= ObjectCreated;
			client.objectCreated -= ObjectCreated;

			serverObj = null;
			clientObj = null;
		}

		[TestMethod]
		public void TestCreate()
		{
			SetupServerBehavior();

			// Make sure that the client get's the network object that was created
			WaitFor(() => { return clientObj != null; });

			Destroy(serverObj);
		}

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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
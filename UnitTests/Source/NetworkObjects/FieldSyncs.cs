using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using UnitTests.NetworkObjects;
using UnitTests.NetworkObjectSetup;

namespace UnitTests.Source.NetworkObjects
{
	[TestClass]
	public class FieldSyncs : BaseNetworkObjectTest
	{
		[ClassInitialize]
		public static void CreateServer(TestContext context)
		{
			NetworkObject.Factory = new NetworkObjectFactory();
		}

		[ClassCleanup]
		public static void DisposeServer()
		{
			ConnectTeardown(ObjectCreated);
		}

        [TestInitialize]
        public void Startup()
        {
            ConnectSetup(objectCreatedCallback: ObjectCreated);
        }

		[TestCleanup]
		public void Cleanup()
		{
			serverObj = null;
			clientObj = null;
			otherClientObj = null;

            ConnectTeardown(ObjectCreated);
        }

		private void SetTargetFields(TestNetworkObject target)
		{
			target.fieldByte = byte.MaxValue;
			target.fieldSByte = sbyte.MinValue;
			target.fieldChar = 'F';
			target.fieldBool = true;
			target.fieldShort = short.MinValue;
			target.fieldUShort = ushort.MaxValue;
			target.fieldInt = int.MinValue;
			target.fieldUInt = uint.MaxValue;
			target.fieldLong = long.MinValue;
			target.fieldULong = ulong.MaxValue;
			target.fieldFloat = float.MaxValue;
			target.fieldDouble = double.MaxValue;
		}

		private bool CheckTargetFields(TestNetworkObject target)
		{
			return target.fieldByte == byte.MaxValue &&
				target.fieldSByte == sbyte.MinValue &&
				target.fieldChar == 'F' &&
				target.fieldBool == true &&
				target.fieldShort == short.MinValue &&
				target.fieldUShort == ushort.MaxValue &&
				target.fieldInt == int.MinValue &&
				target.fieldUInt == uint.MaxValue &&
				target.fieldLong == long.MinValue &&
				target.fieldULong == ulong.MaxValue &&
				target.fieldFloat == float.MaxValue &&
				target.fieldDouble == double.MaxValue;
		}

		[TestMethod]
		public void SetFields()
		{
			SetupClientBehavior();

			SetTargetFields(clientObj);

			WaitFor(() => { return CheckTargetFields(serverObj); });

			Destroy(clientObj);
		}

		[TestMethod]
		public void NotOwnerSetFields()
		{
			SetupServerBehavior();

			SetTargetFields(clientObj);

			// Give the server some time to receive the message
			Thread.Sleep(1000);

			WaitFor(() => { return !CheckTargetFields(serverObj); });

			Destroy(serverObj);
		}

		[TestMethod]
		public void DoubleClientSetFields()
		{
			OtherClientConnectSetup(ObjectCreated);
			SetupClientBehavior();
			SetTargetFields(clientObj);
			WaitFor(() => { return CheckTargetFields(serverObj) && CheckTargetFields(otherClientObj); });
			Destroy(serverObj);
		}

		[TestMethod]
		public void DoubleClientNotOwnerSetFields()
		{
			OtherClientConnectSetup(ObjectCreated);
			SetupServerBehavior();
			SetTargetFields(clientObj);

			// Give the server some time to receive the message
			Thread.Sleep(1000);

			WaitFor(() => { return !CheckTargetFields(serverObj) && !CheckTargetFields(otherClientObj); });
			Destroy(serverObj);
		}
	}
}
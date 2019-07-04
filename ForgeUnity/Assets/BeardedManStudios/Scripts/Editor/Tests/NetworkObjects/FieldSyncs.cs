using System.Threading;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests.NetworkObjects
{
	[TestFixture]
	public class FieldSyncs : BaseNetworkObjectTest
	{
		[OneTimeSetUp]
		public static void CreateServer()
		{
			NetworkObject.Factory = new NetworkObjectFactory();
		}

		[OneTimeTearDown]
		public static void DisposeServer()
		{
			ConnectTeardown(ObjectCreated);
		}

		[SetUp]
		public void Startup()
		{
			ConnectSetup(objectCreatedCallback: ObjectCreated);
		}

		[TearDown]
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
			//target.fieldSByte = sbyte.MinValue;
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
				//target.fieldSByte == sbyte.MinValue &&
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

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void SetFields()
		{
			SetupClientBehavior();

			SetTargetFields(clientObj);

			WaitFor(() => { return CheckTargetFields(serverObj); });

			Destroy(clientObj);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void NotOwnerSetFields()
		{
			SetupServerBehavior();

			SetTargetFields(clientObj);

			// Give the server some time to receive the message
			Thread.Sleep(1000);

			WaitFor(() => { return !CheckTargetFields(serverObj); });

			Destroy(serverObj);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
		public void DoubleClientSetFields()
		{
			OtherClientConnectSetup(ObjectCreated);
			SetupClientBehavior();
			SetTargetFields(clientObj);
			WaitFor(() => { return CheckTargetFields(serverObj) && CheckTargetFields(otherClientObj); });
			Destroy(serverObj);
		}

		[Test]
		[Ignore("Test is awaiting review and refactor")]
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

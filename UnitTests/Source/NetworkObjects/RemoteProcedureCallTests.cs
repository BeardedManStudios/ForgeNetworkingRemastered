using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UnitTests.NetworkObjectSetup;

namespace UnitTests.NetworkObjects
{
    [TestClass]
    public class RemoteProcedureCallTests : BaseNetworkObjectTest
    {
        [ClassInitialize]
        public static void CreateServer(TestContext context)
        {
            NetworkObject.Factory = new NetworkObjectFactory();
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

            ConnectTeardown(ObjectCreated);
        }

        private void CheckValues<T>(object clientObj, object serverObj, T expectedValue)
        {
            Assert.AreEqual(typeof(T), clientObj.GetType());
            Assert.AreEqual(typeof(T), serverObj.GetType());

            Assert.AreEqual(expectedValue, clientObj);
            Assert.AreEqual(expectedValue, serverObj);
        }

        [TestMethod]
        public void BlankRpcTest()
        {
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.All);
            WaitFor(() => { return clientBehavior.calledBlank && serverBehavior.calledBlank; });
            Assert.IsNull(clientBehavior.objectBlank);
            Assert.IsNull(serverBehavior.objectBlank);
            Destroy(clientObj);
        }

        [TestMethod]
        public void BoolRpcTest()
        {
            bool val = true;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BOOL, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledBool && serverBehavior.calledBool; });
            CheckValues(clientBehavior.objectBool, serverBehavior.objectBool, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void ByteRpcTest()
        {
            byte val = 9;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BYTE, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledByte && serverBehavior.calledByte; });
            CheckValues(clientBehavior.objectByte, serverBehavior.objectByte, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void ByteArrayRpcTest()
        {
            byte[] val = new byte[] { 1, 3, 5, 7, 10, 32 };
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BYTE_ARRAY, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledByteArray && serverBehavior.calledByteArray; });

            Assert.AreEqual(typeof(byte[]), clientBehavior.objectByteArray.GetType());
            Assert.AreEqual(typeof(byte[]), serverBehavior.objectByteArray.GetType());

            byte[] clientArr = (byte[])clientBehavior.objectByteArray;
            byte[] serverArr = (byte[])serverBehavior.objectByteArray;

            for (int i = 0; i < val.Length; i++)
            {
                Assert.AreEqual(val[i], clientArr[i]);
                Assert.AreEqual(val[i], serverArr[i]);
            }

            Destroy(clientObj);
        }

        [TestMethod]
        public void CharRpcTest()
        {
            char val = 'F';
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_CHAR, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledChar && serverBehavior.calledChar; });
            CheckValues(clientBehavior.objectChar, serverBehavior.objectChar, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void DoubleRpcTest()
        {
            double val = 9.939;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_DOUBLE, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledDouble && serverBehavior.calledDouble; });
            CheckValues(clientBehavior.objectDouble, serverBehavior.objectDouble, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void FloatRpcTest()
        {
            float val = 9.393f;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_FLOAT, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledFloat && serverBehavior.calledFloat; });
            CheckValues(clientBehavior.objectFloat, serverBehavior.objectFloat, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void IntRpcTest()
        {
            int val = 993;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_INT, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledInt && serverBehavior.calledInt; });
            CheckValues(clientBehavior.objectInt, serverBehavior.objectInt, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void LongRpcTest()
        {
            long val = -939393;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_LONG, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledLong && serverBehavior.calledLong; });
            CheckValues(clientBehavior.objectLong, serverBehavior.objectLong, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void ShortRpcTest()
        {
            short val = 912;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_SHORT, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledShort && serverBehavior.calledShort; });
            CheckValues(clientBehavior.objectShort, serverBehavior.objectShort, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void StringRpcTest()
        {
            string val = "Hello World!";
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_STRING, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledString && serverBehavior.calledString; });
            CheckValues(clientBehavior.objectString, serverBehavior.objectString, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void UIntRpcTest()
        {
            uint val = 9876;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_U_INT, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledUInt && serverBehavior.calledUInt; });
            CheckValues(clientBehavior.objectUInt, serverBehavior.objectUInt, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void ULongRpcTest()
        {
            ulong val = 973453;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_U_LONG, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledULong && serverBehavior.calledULong; });
            CheckValues(clientBehavior.objectULong, serverBehavior.objectULong, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void UShortRpcTest()
        {
            ushort val = 9;
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_U_SHORT, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledUShort && serverBehavior.calledUShort; });
            CheckValues(clientBehavior.objectUShort, serverBehavior.objectUShort, val);
            Destroy(clientObj);
        }

        [TestMethod]
        public void StringAndByteArrayRpcTest()
        {
            string strVal = "Hello World!";

            byte[] byteArrVal = new byte[32];

            System.Random rand = new System.Random();
            for (int i = 0; i < byteArrVal.Length; i++)
                byteArrVal[i] = (byte)rand.Next(0, byte.MaxValue);

            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_STRING_BYTE_ARRAY, Receivers.All, strVal, byteArrVal);
            WaitFor(() => { return clientBehavior.calledByteArray && serverBehavior.calledByteArray && clientBehavior.calledString && serverBehavior.calledString; });

            CheckValues(clientBehavior.objectString, serverBehavior.objectString, strVal);
            Assert.AreEqual(typeof(byte[]), clientBehavior.objectByteArray.GetType());
            Assert.AreEqual(typeof(byte[]), serverBehavior.objectByteArray.GetType());

            byte[] clientArr = (byte[])clientBehavior.objectByteArray;
            byte[] serverArr = (byte[])serverBehavior.objectByteArray;

            for (int i = 0; i < byteArrVal.Length; i++)
            {
                Assert.AreEqual(byteArrVal[i], clientArr[i]);
                Assert.AreEqual(byteArrVal[i], serverArr[i]);
            }

            Destroy(clientObj);
        }

        [TestMethod]
        public void AllRpcTest()
        {
            object[] val = new object[]
            {
                (byte)1,
                (char)'F',
                (short)2,
                (ushort)3,
                (bool)true,
                (int)4,
                (uint)5,
                (float)6.321f,
                (long)7,
                (ulong)8,
                (double)9.93921,
                (string)"Hello World!",
                (byte[])new byte[] { 1, 3, 5, 9, 10, 11, 32 }
            };

            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_ALL, Receivers.All, val);
            WaitFor(() => { return clientBehavior.calledAll && serverBehavior.calledAll; });

            object[] clientVals = (object[])clientBehavior.objectAll;
            object[] serverVals = (object[])clientBehavior.objectAll;

            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] is byte[])
                {
                    byte[] clientArr = (byte[])clientVals[i];
                    byte[] serverArr = (byte[])serverVals[i];

                    for (int j = 0; j < ((byte[])val[i]).Length; j++)
                    {
                        Assert.AreEqual(((byte[])val[i])[j], clientArr[j]);
                        Assert.AreEqual(((byte[])val[i])[j], serverArr[j]);
                    }

                    continue;
                }

                Assert.AreEqual(val[i], clientVals[i]);
                Assert.AreEqual(val[i], serverVals[i]);
            }

            Destroy(clientObj);
        }

        [TestMethod]
        public void UnreliableRpcTest()
        {
            SetupClientBehavior();
            clientObj.SendRpcUnreliable(TestBehavior.RPC_FUNC_BLANK, Receivers.All);
            WaitFor(() => { return clientBehavior.calledBlank && serverBehavior.calledBlank; });
            Assert.IsNull(clientBehavior.objectBlank);
            Assert.IsNull(serverBehavior.objectBlank);
            Destroy(clientObj);
        }

        [TestMethod]
        public void CallMulti()
        {
            int count = 100;
            SetupClientBehavior();
            for (int i = 0; i < 100; i++)
                clientObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.All);

            WaitFor(() =>
            {
                return clientBehavior.calledBlank && serverBehavior.calledBlank &&
                    clientBehavior.countBlank == count && serverBehavior.countBlank == count;
            });

            Assert.IsNull(clientBehavior.objectBlank);
            Assert.IsNull(serverBehavior.objectBlank);
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversServerOnly()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.Server);
            WaitFor(() => { return !clientBehavior.calledBlank && !otherClientBehavior.calledBlank && serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversServerToOwnerOnly()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            serverObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.Owner);
            WaitFor(() => { return clientBehavior.calledBlank && !otherClientBehavior.calledBlank && !serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversClientToOwnerOnly()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.Owner);
            WaitFor(() => { return clientBehavior.calledBlank && !otherClientBehavior.calledBlank && !serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversOwnerAndServerOnly()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.ServerAndOwner);
            WaitFor(() => { return clientBehavior.calledBlank && !otherClientBehavior.calledBlank && serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversServerToTargetOnly()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            serverObj.SendRpc(server.Players.ElementAt(1), TestBehavior.RPC_FUNC_BLANK);
            WaitFor(() => { return clientBehavior.calledBlank && !otherClientBehavior.calledBlank && !serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversServerToOtherTargetOnly()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            serverObj.SendRpc(server.Players.Last(), TestBehavior.RPC_FUNC_BLANK);
            WaitFor(() => { return !clientBehavior.calledBlank && otherClientBehavior.calledBlank && !serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversOthersOnlyFromServer()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            serverObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.Others);
            WaitFor(() => { return clientBehavior.calledBlank && otherClientBehavior.calledBlank && !serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversOthersOnlyFromClient()
        {
            OtherClientConnectSetup(ObjectCreated);
            SetupClientBehavior();
            clientObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.Others);
            WaitFor(() => { return !clientBehavior.calledBlank && otherClientBehavior.calledBlank && serverBehavior.calledBlank; });
            Destroy(clientObj);
        }

        [TestMethod]
        public void ReceiversAllProximityFromServer()
        {
            server.ProximityDistance = 5.0f;
            server.Me.ProximityLocation = new BeardedManStudios.Vector(4.5f, 0, 0);
            server.Players.Last().ProximityLocation = new BeardedManStudios.Vector(0, 0, 0);

            SetupServerBehavior();
            serverObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.AllProximity);
            WaitFor(() => { return clientBehavior.calledBlank && serverBehavior.calledBlank; });
            Destroy(serverObj);
        }

        [TestMethod]
        public void ReceiversAllProximityTooFarFromServer()
        {
            server.ProximityDistance = 5.0f;
            server.Me.ProximityLocation = new BeardedManStudios.Vector(10, 0, 0);
            server.Players.Last().ProximityLocation = new BeardedManStudios.Vector(0, 0, 0);

            SetupServerBehavior();
            serverObj.SendRpc(TestBehavior.RPC_FUNC_BLANK, Receivers.AllProximity);
            WaitFor(() => { return !clientBehavior.calledBlank && serverBehavior.calledBlank; });
            Destroy(serverObj);
        }
    }
}
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System;
using UnitTests.Source.UDP;

namespace UnitTests.NetworkObjects
{
	public class BaseNetworkObjectTest : BaseUDPTests
	{
		protected static TestNetworkObject serverObj, clientObj, otherClientObj;
		protected static RPCBehavior serverBehavior, clientBehavior, otherClientBehavior;

		protected static void ObjectCreated(NetworkObject target)
		{
			if (target is TestNetworkObject)
			{
				if (target.Networker.IsServer)
					serverObj = (TestNetworkObject)target;
				else
				{
					if (target.Networker == client)
						clientObj = (TestNetworkObject)target;
					else
						otherClientObj = (TestNetworkObject)target;
				}

				target.ReleaseCreateBuffer();
			}
		}

		protected void Destroy(TestNetworkObject obj)
		{
			obj.Destroy();

			// Make sure that the object was destroyed on the server and the client
			if (otherClient == null)
				WaitFor(() => { return server.NetworkObjectList.Count == 0 && client.NetworkObjectList.Count == 0; });
			else
			{
				WaitFor(() =>
				{
					return server.NetworkObjectList.Count == 0 &&
					client.NetworkObjectList.Count == 0 &&
					otherClient.NetworkObjectList.Count == 0;
				});
			}

			serverBehavior = null;
			clientBehavior = null;
			otherClientBehavior = null;
			serverObj = null;
			clientObj = null;
			otherClientObj = null;
		}

		protected void SetupClientBehavior()
		{
			// Make the network object on the client and make sure that the server creates it too
			clientBehavior = new RPCBehavior();
			clientBehavior.Initialize(client);
			clientObj = clientBehavior.networkObject;

			WaitFor(() => { return serverObj != null; });
			serverBehavior = new RPCBehavior();
			serverBehavior.Initialize(serverObj);

			if (otherClient == null)
			{
				WaitFor(() => { return client.NetworkObjects.Count == 1 && server.NetworkObjects.Count == 1; });
				return;
			}

			WaitFor(() => { return otherClientObj != null; });
			otherClientBehavior = new RPCBehavior();
			otherClientBehavior.Initialize(otherClientObj);

			WaitFor(() =>
			{
				return client.NetworkObjects.Count == 1 &&
				otherClient.NetworkObjects.Count == 1 &&
				server.NetworkObjects.Count == 1;
			});
		}

		protected void SetupServerBehavior()
		{
			// Make the network object on the client and make sure that the server creates it too
			serverBehavior = new RPCBehavior();
			serverBehavior.Initialize(server);
			serverObj = serverBehavior.networkObject;

			WaitFor(() => { return clientObj != null; });
			clientBehavior = new RPCBehavior();
			clientBehavior.Initialize(clientObj);

			if (otherClient == null)
			{
				WaitFor(() => { return client.NetworkObjects.Count == 1 && server.NetworkObjects.Count == 1; });
				return;
			}

			WaitFor(() => { return otherClientObj != null; });
			otherClientBehavior = new RPCBehavior();
			otherClientBehavior.Initialize(otherClientObj);

			WaitFor(() =>
			{
				return client.NetworkObjects.Count == 1 &&
				otherClient.NetworkObjects.Count == 1 &&
				server.NetworkObjects.Count == 1;
			});
		}

		protected class RPCBehavior : TestBehavior
		{
			public bool calledAll,
				calledBlank,
				calledBool,
				calledByte,
				calledByteArray,
				calledChar,
				calledDouble,
				calledFloat,
				calledInt,
				calledLong,
				calledShort,
				calledString,
				calledUInt,
				calledULong,
				calledUShort;

			public int countAll,
				countBlank,
				countBool,
				countByte,
				countByteArray,
				countChar,
				countDouble,
				countFloat,
				countInt,
				countLong,
				countShort,
				countString,
				countUInt,
				countULong,
				countUShort;

			public object objectAll,
				objectBlank,
				objectBool,
				objectByte,
				objectByteArray,
				objectChar,
				objectDouble,
				objectFloat,
				objectInt,
				objectLong,
				objectShort,
				objectString,
				objectUInt,
				objectULong,
				objectUShort;

			public override void FuncAll(RpcArgs args)
			{
				calledAll = true;
				countAll++;
				object[] tmp = new object[]
				{
					args.GetNext<byte>(),
					args.GetNext<char>(),
					args.GetNext<short>(),
					args.GetNext<ushort>(),
					args.GetNext<bool>(),
					args.GetNext<int>(),
					args.GetNext<uint>(),
					args.GetNext<float>(),
					args.GetNext<long>(),
					args.GetNext<ulong>(),
					args.GetNext<double>(),
					args.GetNext<string>(),
					args.GetNext<byte[]>()
				};

				objectAll = tmp;
			}

			public override void FuncBlank(RpcArgs args)
			{
				Console.WriteLine("Calling blank on " + (networkObject.IsServer ? "Server" : "Client"));

				calledBlank = true;
				countBlank++;
				objectBlank = null;
			}

			public override void FuncBool(RpcArgs args)
			{
				calledBool = true;
				countBool++;
				objectBool = args.GetNext<bool>();
			}

			public override void FuncByte(RpcArgs args)
			{
				calledByte = true;
				countByte++;
				objectByte = args.GetNext<byte>();
			}

			public override void FuncByteArray(RpcArgs args)
			{
				calledByteArray = true;
				countByteArray++;
				objectByteArray = args.GetNext<byte[]>();
			}

			public override void FuncChar(RpcArgs args)
			{
				calledChar = true;
				countChar++;
				objectChar = args.GetNext<char>();
			}

			public override void FuncDouble(RpcArgs args)
			{
				calledDouble = true;
				countDouble++;
				objectDouble = args.GetNext<double>();
			}

			public override void FuncFloat(RpcArgs args)
			{
				calledFloat = true;
				countFloat++;
				objectFloat = args.GetNext<float>();
			}

			public override void FuncInt(RpcArgs args)
			{
				calledInt = true;
				countInt++;
				objectInt = args.GetNext<int>();
			}

			public override void FuncLong(RpcArgs args)
			{
				calledLong = true;
				countLong++;
				objectLong = args.GetNext<long>();
			}

			public override void FuncShort(RpcArgs args)
			{
				calledShort = true;
				countShort++;
				objectShort = args.GetNext<short>();
			}

			public override void FuncString(RpcArgs args)
			{
				calledString = true;
				countString++;
				objectString = args.GetNext<string>();
			}

			public override void FuncUInt(RpcArgs args)
			{
				calledUInt = true;
				countUInt++;
				objectUInt = args.GetNext<uint>();
			}

			public override void FuncULong(RpcArgs args)
			{
				calledULong = true;
				countULong++;
				objectULong = args.GetNext<ulong>();
			}

			public override void FuncUShort(RpcArgs args)
			{
				calledUShort = true;
				countUShort++;
				objectUShort = args.GetNext<ushort>();
			}
		}
	}
}
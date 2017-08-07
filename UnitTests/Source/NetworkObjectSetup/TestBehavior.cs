namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[][\"byte\"][\"char\"][\"short\"][\"ushort\"][\"bool\"][\"int\"][\"uint\"][\"float\"][\"long\"][\"ulong\"][\"double\"][\"string\"][\"byte[]\"][\"byte\", \"char\", \"short\", \"ushort\", \"bool\", \"int\", \"uint\", \"float\", \"long\", \"ulong\", \"double\", \"string\", \"byte[]\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[][\"argByte\"][\"argChar\"][\"argShort\"][\"argUShort\"][\"argBool\"][\"argInt\"][\"argUInt\"][\"argFloat\"][\"argLong\"][\"argULong\"][\"argDouble\"][\"argString\"][\"argByteArray\"][\"argByte\", \"argChar\", \"argShort\", \"argUShort\", \"argBool\", \"argInt\", \"argUInt\", \"argFloat\", \"argLong\", \"argULong\", \"argDouble\", \"argString\", \"argByteArray\"]]")]
	public abstract partial class TestBehavior : INetworkBehavior
	{
		public const byte RPC_FUNC_BLANK = 0 + 4;
		public const byte RPC_FUNC_BYTE = 1 + 4;
		public const byte RPC_FUNC_CHAR = 2 + 4;
		public const byte RPC_FUNC_SHORT = 3 + 4;
		public const byte RPC_FUNC_U_SHORT = 4 + 4;
		public const byte RPC_FUNC_BOOL = 5 + 4;
		public const byte RPC_FUNC_INT = 6 + 4;
		public const byte RPC_FUNC_U_INT = 7 + 4;
		public const byte RPC_FUNC_FLOAT = 8 + 4;
		public const byte RPC_FUNC_LONG = 9 + 4;
		public const byte RPC_FUNC_U_LONG = 10 + 4;
		public const byte RPC_FUNC_DOUBLE = 11 + 4;
		public const byte RPC_FUNC_STRING = 12 + 4;
		public const byte RPC_FUNC_BYTE_ARRAY = 13 + 4;
		public const byte RPC_FUNC_ALL = 14 + 4;
		public const byte RPC_FUNC_STRING_BYTE_ARRAY = 15 + 4;

		public int TempAttachCode { get; set; }

		public TestNetworkObject networkObject = null;

		public void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;

			networkObject = (TestNetworkObject)obj;

			networkObject.RegisterRpc("FuncBlank", FuncBlank);
			networkObject.RegisterRpc("FuncByte", FuncByte, typeof(byte));
			networkObject.RegisterRpc("FuncChar", FuncChar, typeof(char));
			networkObject.RegisterRpc("FuncShort", FuncShort, typeof(short));
			networkObject.RegisterRpc("FuncUShort", FuncUShort, typeof(ushort));
			networkObject.RegisterRpc("FuncBool", FuncBool, typeof(bool));
			networkObject.RegisterRpc("FuncInt", FuncInt, typeof(int));
			networkObject.RegisterRpc("FuncUInt", FuncUInt, typeof(uint));
			networkObject.RegisterRpc("FuncFloat", FuncFloat, typeof(float));
			networkObject.RegisterRpc("FuncLong", FuncLong, typeof(long));
			networkObject.RegisterRpc("FuncULong", FuncULong, typeof(ulong));
			networkObject.RegisterRpc("FuncDouble", FuncDouble, typeof(double));
			networkObject.RegisterRpc("FuncString", FuncString, typeof(string));
			networkObject.RegisterRpc("FuncByteArray", FuncByteArray, typeof(byte[]));
			networkObject.RegisterRpc("FuncAll", FuncAll, typeof(byte), typeof(char), typeof(short), typeof(ushort), typeof(bool), typeof(int), typeof(uint), typeof(float), typeof(long), typeof(ulong), typeof(double), typeof(string), typeof(byte[]));
			networkObject.RegisterRpc("FuncStringByteArray", FuncStringByteArray, typeof(string), typeof(byte[]));

			networkObject.onDestroy += DestroyGameObject;
		}

		public void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new TestNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			networkObject.onDestroy -= DestroyGameObject;
		}

		public NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new TestNetworkObject(networker, this, createCode, metadata);
		}

		protected void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncBlank(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncByte(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncChar(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncShort(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncUShort(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncBool(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncInt(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncUInt(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncFloat(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncLong(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncULong(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncDouble(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncString(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncByteArray(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void FuncAll(RpcArgs args);

		public abstract void FuncStringByteArray(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
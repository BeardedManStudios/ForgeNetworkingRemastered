namespace BeardedManStudios.Forge.Networking
{
	public struct RpcArgs
	{
		public object[] Args { get; set; }
		public RPCInfo Info { get; private set; }
		public int ReadIndex { get; set; }

		public RpcArgs(object[] args, RPCInfo info) : this()
		{
			this.Args = args;
			this.Info = info;
			this.ReadIndex = 0;
		}

		public T GetNext<T>()
		{
			return (T)Args[ReadIndex++];
		}

		public T GetAt<T>(int index)
		{
			return (T)Args[index];
		}
	}
}

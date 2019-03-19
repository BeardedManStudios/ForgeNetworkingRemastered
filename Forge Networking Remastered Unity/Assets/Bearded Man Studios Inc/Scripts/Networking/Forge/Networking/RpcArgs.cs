/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

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
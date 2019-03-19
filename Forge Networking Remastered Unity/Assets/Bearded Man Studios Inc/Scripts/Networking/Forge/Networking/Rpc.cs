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

using BeardedManStudios.Source.Threading;
using System;
using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking
{
	public class Rpc
	{
		public static IThreadRunner MainThreadRunner { get; set; }

		public byte Id { get; private set; }

		public int ArgumentCount { get; private set; }

		private Action<RpcArgs> callback = null;
		private Type[] argumentTypes = null;

		public Rpc(Action<RpcArgs> callback, Type[] argumentTypes)
		{
			ArgumentCount = argumentTypes.Length;
			this.callback = callback;
			this.argumentTypes = argumentTypes;
		}

		public object[] ReadArgs(BMSByte data)
		{
			List<object> arguments = new List<object>();
			foreach (Type t in argumentTypes)
				arguments.Add(ObjectMapper.Instance.Map(t, data));

			return arguments.ToArray();
		}

		public void Invoke(RpcArgs rpcArgs, bool skipMainThreadRunner = false)
		{
			if (MainThreadRunner != null && !skipMainThreadRunner)
				MainThreadRunner.Execute(() => { callback(rpcArgs); });
			else
				callback(rpcArgs);
		}

		public void ValidateParameters(object[] arguments)
		{
			if (arguments.Length != argumentTypes.Length)
			{
				string argTypes = "";

				for (int i = 0; i < arguments.Length; i++)
				{
					if (!string.IsNullOrEmpty(argTypes))
					{
						argTypes += ", ";
					}

					argTypes += arguments[i].GetType();
				}

				throw new BaseNetworkException("There are " + arguments.Length + " supplied arguments, but this Rpc expects " + argumentTypes.Length + ". Args: " + argTypes);
			}

			for (int i = 0; i < arguments.Length; i++)
			{
				if (arguments[i].GetType() != argumentTypes[i])
					throw new BaseNetworkException("The argument with index " + i + " was expected to be a " + argumentTypes[i] + " but got " + arguments[i].GetType() + " instead");
			}
		}
	}
}
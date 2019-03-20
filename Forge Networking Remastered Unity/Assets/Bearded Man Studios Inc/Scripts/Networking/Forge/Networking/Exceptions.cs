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

using System;

namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// The base network exception that all Forge.Networking exceptions should derive from
	/// </summary>
	public class BaseNetworkException : Exception
	{
		public BaseNetworkException() : base() { }
		public BaseNetworkException(string message) : base(message) { }
#if !WINDOWS_UWP
		public BaseNetworkException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
		public BaseNetworkException(string message, Exception innerException) : base(message, innerException) { }
	}

	/// <summary>
	/// There was an issue binidng the listener to the socket
	/// </summary>
	public class FailedBindingException : BaseNetworkException
	{
		public FailedBindingException() : base() { }
		public FailedBindingException(string message) : base(message) { }
#if !WINDOWS_UWP
		public FailedBindingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
		public FailedBindingException(string message, Exception innerException) : base(message, innerException) { }
	}
}
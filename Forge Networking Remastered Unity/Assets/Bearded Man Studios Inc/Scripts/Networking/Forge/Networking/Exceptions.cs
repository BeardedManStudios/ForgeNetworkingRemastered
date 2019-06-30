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
		public BaseNetworkException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		public BaseNetworkException(string message, Exception innerException) : base(message, innerException) { }
	}

	/// <summary>
	/// There was an issue binidng the listener to the socket
	/// </summary>
	public class FailedBindingException : BaseNetworkException
	{
		public FailedBindingException() : base() { }
		public FailedBindingException(string message) : base(message) { }
		public FailedBindingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		public FailedBindingException(string message, Exception innerException) : base(message, innerException) { }
	}
}

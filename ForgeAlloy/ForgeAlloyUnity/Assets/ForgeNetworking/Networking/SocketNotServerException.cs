using System;

namespace Forge.Networking
{
	[Serializable]
	public class SocketNotServerException : Exception
	{
		public SocketNotServerException(string message) : base(message)
		{

		}
	}
}

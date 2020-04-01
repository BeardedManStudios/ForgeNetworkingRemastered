using System;

namespace Forge.Networking.Messaging
{
	public class MessageCodeNotFoundException : Exception
	{
		public MessageCodeNotFoundException(int code)
			: base($"There was no type found with the code {code}")
		{
		}
	}
}

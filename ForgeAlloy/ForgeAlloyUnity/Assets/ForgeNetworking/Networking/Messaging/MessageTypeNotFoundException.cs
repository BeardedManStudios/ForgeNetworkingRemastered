using System;

namespace Forge.Networking.Messaging
{
	public class MessageTypeNotFoundException : Exception
	{
		public MessageTypeNotFoundException(Type type)
			: base($"The message type of {type} was never registered")
		{
		}
	}
}

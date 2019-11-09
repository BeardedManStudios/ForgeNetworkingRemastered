using System;

namespace Forge.Networking.Messaging
{
	public class DuplicateMessageTypeRegistrationException : Exception
	{
		public DuplicateMessageTypeRegistrationException(Type t)
			: base($"The type {t} has already been registered in the type/code lookup")
		{
		}
	}
}

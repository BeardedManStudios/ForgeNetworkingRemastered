using System;

namespace Forge.Networking.Messaging
{
	public class MessageRepositoryMissingGuidOnMessageException : Exception
	{
		public MessageRepositoryMissingGuidOnMessageException()
			: base($"A message requires a receipt with a valid signature to be stored and located in the message repository")
		{

		}
	}
}

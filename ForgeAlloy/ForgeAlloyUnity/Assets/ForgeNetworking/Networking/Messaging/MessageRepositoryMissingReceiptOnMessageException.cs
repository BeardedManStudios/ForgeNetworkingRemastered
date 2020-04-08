using System;

namespace Forge.Networking.Messaging
{
	public class MessageRepositoryMissingReceiptOnMessageException : Exception
	{
		public MessageRepositoryMissingReceiptOnMessageException()
			: base($"A message requires a receipt with a valid signature to be stored and located in the message repository")
		{

		}
	}
}

using System;

namespace Forge.Networking.Messaging
{
	public class MessageWithReceiptSignatureAlreadyExistsException : Exception
	{
		public MessageWithReceiptSignatureAlreadyExistsException()
			: base($"A message with the same receipt signature exists in the repository and this must be unique")
		{

		}
	}
}

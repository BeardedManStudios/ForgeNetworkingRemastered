using System;

namespace Forge.Networking.Messaging.Paging
{
	public class CantBreakdownEmptyMessageException : Exception
	{
		public CantBreakdownEmptyMessageException()
			: base("An empty message buffer was supplied to the message destructor")
		{

		}
	}
}

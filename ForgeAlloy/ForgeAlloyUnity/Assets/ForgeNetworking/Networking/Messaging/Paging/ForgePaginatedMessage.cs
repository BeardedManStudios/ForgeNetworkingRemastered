using System.Collections.Generic;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgePaginatedMessage : IPaginatedMessage
	{
		public BMSByte Buffer { get; set; }
		public int TotalSize => Buffer.Size;
		public List<IMessagePage> Pages => _pages;

		private readonly List<IMessagePage> _pages = new List<IMessagePage>();
	}
}

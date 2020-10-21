using System.Collections.Generic;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IPaginatedMessage
	{
		BMSByte Buffer { get; set; }
		int TotalSize { get; }
		List<IMessagePage> Pages { get; }
	}
}

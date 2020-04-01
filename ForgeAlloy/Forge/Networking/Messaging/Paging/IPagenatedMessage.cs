using System.Collections.Generic;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IPagenatedMessage
	{
		BMSByte Buffer { get; set; }
		int TotalSize { get; }
		List<IMessagePage> Pages { get; }
	}
}

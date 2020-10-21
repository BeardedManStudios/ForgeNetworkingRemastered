using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IMessageDestructor
	{
		int HeaderLength { get; }
		int MaxPageLength { get; }
		BMSBytePool BufferPool { set; }
		IPaginatedMessage BreakdownMessage(BMSByte messageBuffer);
	}
}

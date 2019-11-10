using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IMessageDestructor
	{
		int HeaderLength { get; }
		int MaxPageLength { get; }
		IPagenatedMessage BreakdownMessage(BMSByte messageBuffer);
	}
}

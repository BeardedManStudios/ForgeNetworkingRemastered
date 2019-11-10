using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IMessageBufferInterpreter
	{
		IMessageConstructor ReconstructPacketPage(BMSByte buffer);
	}
}

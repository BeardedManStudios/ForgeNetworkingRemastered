using System.Net;
using Forge.Networking.Players;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IMessageBufferInterpreter
	{
		IMessageConstructor ReconstructPacketPage(BMSByte buffer, EndPoint sender);
		void ClearBufferFor(INetPlayer player);
		void Release(IMessageConstructor constructor);
	}
}

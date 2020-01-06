using System.Net;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IMessageConstructor
	{
		EndPoint Sender { get; }
		bool MessageReconstructed { get; }
		BMSByte MessageBuffer { get; }
		void Setup(BMSBytePool bufferPool);
		void Teardown();
		void ReconstructMessagePage(BMSByte page, EndPoint sender);
	}
}

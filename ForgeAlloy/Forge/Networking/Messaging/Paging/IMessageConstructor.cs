using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public interface IMessageConstructor
	{
		bool MessageReconstructed { get; }
		BMSByte MessageBuffer { get; }
		void ReconstructMessagePage(BMSByte part);
	}
}

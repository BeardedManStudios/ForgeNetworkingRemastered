using Forge.DataStructures;

namespace Forge.Networking.Messaging
{
	public interface IMessageReceipt
	{
		ISignature Signature { get; set; }
	}
}

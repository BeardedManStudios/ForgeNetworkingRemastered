using Forge.DataStructures;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageReceipt : ForgeSignature, IMessageReceiptSignature
	{
		public ForgeMessageReceipt(ISignatureGenerator<int> generator)
			: base(generator)
		{

		}
	}
}

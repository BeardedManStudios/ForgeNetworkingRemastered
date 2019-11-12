using System;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageReceipt : IMessageReceipt
	{
		public Guid Signature { get; set; }

		public ForgeMessageReceipt()
		{
			Signature = Guid.NewGuid();
		}
	}
}

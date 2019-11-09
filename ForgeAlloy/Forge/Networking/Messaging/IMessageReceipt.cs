using System;

namespace Forge.Networking.Messaging
{
	public interface IMessageReceipt
	{
		Guid Signature { get; set; }
	}
}

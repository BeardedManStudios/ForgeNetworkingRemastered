using System.Net;

namespace Forge.Networking.Messaging
{
	public interface IMessageRepeater
	{
		int RepeatMillisecondsInterval { get; set; }
		void Start(INetworkMediator networkMediator);
		void AddMessageToRepeat(IMessage message, EndPoint receiver);
		void RemoveRepeatingMessage(EndPoint sender, IMessageReceiptSignature messageReceipt);
		void RemoveAllFor(EndPoint receiver);
	}
}

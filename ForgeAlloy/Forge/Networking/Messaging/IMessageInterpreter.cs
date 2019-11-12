using System.Net;

namespace Forge.Networking.Messaging
{
	public interface IMessageInterpreter
	{
		bool ValidOnClient { get; }
		bool ValidOnServer { get; }
		void Interpret(INetworkContainer netContainer, EndPoint sender, IMessage message);
	}
}

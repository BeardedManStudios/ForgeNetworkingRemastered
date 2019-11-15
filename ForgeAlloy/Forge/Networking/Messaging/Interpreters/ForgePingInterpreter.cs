using System.Net;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgePingInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netContainer, EndPoint sender, IMessage message)
		{
			// Nothing to do here, we have already updated the player timestamp
		}
	}
}

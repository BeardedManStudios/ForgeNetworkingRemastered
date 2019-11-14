using System.Net;
using Forge.Networking;
using Forge.Networking.Messaging;

namespace Forge.ServerRegistry.Messaging.Interpreters
{
	public class ForgeServerRegistryInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netContainer, EndPoint sender, IMessage message)
		{
			throw new System.NotImplementedException();
		}
	}
}

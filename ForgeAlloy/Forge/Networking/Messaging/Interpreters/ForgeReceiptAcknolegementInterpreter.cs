using System;
using System.Net;

namespace Forge.Networking.Messaging.Interpreters
{
	public class ForgeReceiptAcknolegementInterpreter : IMessageInterpreter
	{
		public bool ValidOnClient => true;
		public bool ValidOnServer => true;

		public void Interpret(INetworkFacade netHost, EndPoint sender, IMessage message)
		{
			throw new NotImplementedException();
		}
	}
}

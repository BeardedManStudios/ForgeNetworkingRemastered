using System;
using System.Net;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.ServerRegistry.Messaging.Interpreters;

namespace ForgeServerRegistryService.Messaging.Interpreters
{
	public class RegisterAsServerInterpreter : IRegisterAsServerInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netContainer, EndPoint sender, IMessage message)
		{
			// TODO:  Register this sender as a server in the players
			throw new NotImplementedException();
		}
	}
}

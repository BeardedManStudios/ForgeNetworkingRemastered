using System;
using System.Net;
using Forge.Networking.Messaging;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SendEntitiesToNewPlayerInterpreter : IMessageInterpreter
	{
		public static SendEntitiesToNewPlayerInterpreter Instance { get; private set; } = new SendEntitiesToNewPlayerInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			// TODO:  This new player needs to spawn all the entities being sent
			throw new NotImplementedException();
		}
	}
}
